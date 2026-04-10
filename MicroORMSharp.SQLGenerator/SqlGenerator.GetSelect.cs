using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        /*
         * Thanks to the following github repository & NuGet package for the basic implementation of the expression parser
         * This is a heavily modified version of the original code
         * https://github.com/gambarra/ExpressionExtensionSQL/tree/master/ExpressionExtensionSQL
         */

        private static readonly IDictionary<ExpressionType, string> _nodeTypeMappings = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Add, "+" },
            { ExpressionType.And, "AND" },
            { ExpressionType.AndAlso, "AND" },
            { ExpressionType.Divide, "/" },
            { ExpressionType.Equal, "=" },
            { ExpressionType.ExclusiveOr, "^" },
            { ExpressionType.GreaterThan, ">" },
            { ExpressionType.GreaterThanOrEqual, ">=" },
            { ExpressionType.LessThan, "<" },
            { ExpressionType.LessThanOrEqual, "<=" },
            { ExpressionType.Modulo, "%" },
            { ExpressionType.Multiply, "*" },
            { ExpressionType.Negate, "-" },
            { ExpressionType.Not, "NOT" },
            { ExpressionType.NotEqual, "<>" },
            { ExpressionType.Or, "OR" },
            { ExpressionType.OrElse, "OR" },
            { ExpressionType.Subtract, "-" }
        };

        public SqlQuery Select(DbQuery<T> dbQuery)
        {
            var sqlQuery = new SqlQuery("SELECT");
            var hasPagination = dbQuery._offset.HasValue && dbQuery._take.HasValue;

            if (DatabaseType == DatabaseType.SqlServer && dbQuery._take != null && !hasPagination)
            {
                sqlQuery.Query.Append($" TOP ({dbQuery._take})");
            }

            var selectColumns = dbQuery._selectClause != null && dbQuery._selectClause.Any() ? dbQuery._selectClause.ToList() : Properties.Select(x => (MemberInfo)x).ToList();
            if (selectColumns.Any())
            {
                sqlQuery.Query.Append($" {string.Join(", ", GenerateSelectClause(TableName, selectColumns))}");
            }

            if (JoinProperties.Any())
            {
                var joinSelectColumns = GenerateJoinSelectClause(JoinProperties, 1);
                if (joinSelectColumns.Any())
                {
                    sqlQuery.Query.Append($", {string.Join(", ", joinSelectColumns)}");
                }
            }

            sqlQuery.Query.Append($" FROM {GetFullTableName()}");

            if (JoinProperties.Any())
            {
                var joins = GenerateJoins(TableName, JoinProperties, 1);
                sqlQuery.Query.Append($"{joins.Query}");
            }

            if (dbQuery._whereClause != null)
            {
                var filter = GenerateWhereClause(dbQuery._whereClause.Body);
                sqlQuery.Parameters = filter.Parameters;
                sqlQuery.Query.Append($" WHERE {filter.ToString()}");
            }

            var orderByClause = GetOrderByClause(dbQuery, hasPagination);
            if (!string.IsNullOrEmpty(orderByClause))
            {
                sqlQuery.Query.Append(" ORDER BY ");
                sqlQuery.Query.Append(orderByClause);
            }

            if (DatabaseType == DatabaseType.MySql)
            {
                if (hasPagination)
                {
                    sqlQuery.Query.Append($" LIMIT {dbQuery._take} OFFSET {dbQuery._offset}");
                }
                else if (dbQuery._take != null)
                {
                    sqlQuery.Query.Append($" LIMIT {dbQuery._take}");
                }
            }

            if (DatabaseType == DatabaseType.SqlServer && hasPagination)
            {
                sqlQuery.Query.Append($" OFFSET {dbQuery._offset} ROWS FETCH NEXT {dbQuery._take} ROWS ONLY");
            }

            return sqlQuery;
        }

        private string GetOrderByClause(DbQuery<T> dbQuery, bool hasPagination)
        {
            if (dbQuery._orderBy.Any())
            {
                return string.Join(", ", dbQuery._orderBy.Select(x => $"{AddBrackets(TableName)}.{AddBrackets(GetPropertyName(x.Key))} {(x.Value ? "DESC" : "ASC")}"));
            }

            if (!hasPagination || DatabaseType != DatabaseType.SqlServer)
            {
                return string.Empty;
            }

            var fallbackProperty = IdentityProperties.FirstOrDefault() ?? Properties.FirstOrDefault()
                ?? throw new InvalidOperationException($"Unable to determine an ORDER BY column for paginated query.");

            return $"{AddBrackets(TableName)}.{AddBrackets(GetPropertyName(fallbackProperty))} ASC";
        }

        private IEnumerable<string> GenerateSelectClause(string tableName, IEnumerable<MemberInfo> memberInfo)
        {
            List<string> columns = new List<string>();

            foreach (var member in memberInfo)
            {
                var classColumn = member.Name;
                var dbColumn = GetPropertyName(member);

                columns.Add($"{AddBrackets(tableName)}.{AddBrackets(dbColumn)} AS {AddBrackets(classColumn)}");
            }

            return columns;
        }

        private IEnumerable<string> GenerateJoinSelectClause(IEnumerable<MemberInfo> joins, int depth)
        {
            EnsureJoinDepth(depth);

            foreach (var join in joins)
            {
                var joinMetadata = GetPropertyMetadata((PropertyInfo)join).Join;
                var joinTypeMetadata = GetMetadata(joinMetadata.JoinedType);
                var joinColumns = GetSelectableMembers(joinMetadata.JoinedType);

                foreach (var column in GenerateSelectClause(joinTypeMetadata.TableName, joinColumns))
                {
                    yield return column;
                }

                foreach (var nestedColumn in GenerateJoinSelectClause(GetJoinMembers(joinMetadata.JoinedType), depth + 1))
                {
                    yield return nestedColumn;
                }
            }
        }

        private SqlQuery GenerateJoins(string parentTableName, IEnumerable<MemberInfo> joins, int depth)
        {
            EnsureJoinDepth(depth);

            SqlQuery sqlQuery = new SqlQuery();

            foreach (var join in joins)
            {
                var joinMetadata = GetPropertyMetadata((PropertyInfo)join).Join;
                var joinTypeMetadata = GetMetadata(joinMetadata.JoinedType);

                sqlQuery.Query.Append($" {GetJoinKeyword(joinMetadata.JoinType)} JOIN {FormatFullTableName(joinTypeMetadata.TableDatabase, joinTypeMetadata.TableSchema, joinTypeMetadata.TableName, DatabaseType)} ON {AddBrackets(joinTypeMetadata.TableName)}.{AddBrackets(joinMetadata.OtherKey)} = {AddBrackets(parentTableName)}.{AddBrackets(joinMetadata.TableKey)}");

                var nestedJoins = GetJoinMembers(joinMetadata.JoinedType);
                if (nestedJoins.Any())
                {
                    sqlQuery.Query.Append(GenerateJoins(joinTypeMetadata.TableName, nestedJoins, depth + 1).Query.ToString());
                }
            }

            return sqlQuery;
        }

        private void EnsureJoinDepth(int depth)
        {
            if (depth > DBJoin.MaxDepth)
            {
                throw new InvalidOperationException($"Nested joins are limited to {DBJoin.MaxDepth} levels.");
            }
        }

        private string GetJoinKeyword(DBJoinType joinType)
        {
            return joinType switch
            {
                DBJoinType.Left => "LEFT",
                DBJoinType.Right => "RIGHT",
                _ => "INNER"
            };
        }

        private IEnumerable<MemberInfo> GetSelectableMembers(Type type)
        {
            return GetMetadata(type).Properties.Select(x => (MemberInfo)x);
        }

        private IEnumerable<MemberInfo> GetJoinMembers(Type type)
        {
            return GetMetadata(type).JoinProperties.Select(x => (MemberInfo)x);
        }

        private SqlQuery GenerateWhereClause(Expression expression)
        {
            int paramCount = 1;
            return ParseExpression(ref paramCount, expression, isUnary: true);
        }

        private SqlQuery ParseExpression(ref int parameterCount, Expression expression, bool isUnary = false,
            string prefix = null, string suffix = null, bool left = true)
        {
            return expression switch
            {
                UnaryExpression unary => UnaryExpressionExtract(ref parameterCount, unary),
                BinaryExpression binary => BinaryExpressionExtract(ref parameterCount, binary),
                ConstantExpression constant => ConstantExpressionExtract(ref parameterCount, constant, isUnary, prefix, suffix, left),
                MemberExpression member => MemberExpressionExtract(ref parameterCount, member, isUnary, prefix, suffix, left),
                MethodCallExpression method => MethodCallExpressionExtract(ref parameterCount, method),
                InvocationExpression invocation => InvocationExpressionExtract(ref parameterCount, invocation, left),
                _ => throw new Exception($"Unsupported expression: {expression.GetType().Name}")
            };
        }

        private SqlQuery InvocationExpressionExtract(ref int i, InvocationExpression expression, bool left)
        {
            return ParseExpression(ref i, ((Expression<Func<T, bool>>)expression.Expression).Body, left: left);
        }

        private SqlQuery MethodCallExpressionExtract(ref int i, MethodCallExpression expression)
        {
            bool IsStringMethod(MethodInfo method, string name) => method == typeof(string).GetMethod(name, new[] { typeof(string) });

            if (IsStringMethod(expression.Method, "Contains"))
            {
                return SqlQuery.Concat(
                    ParseExpression(ref i, expression.Object),
                    "LIKE",
                    ParseExpression(ref i, expression.Arguments[0], prefix: "%", suffix: "%")
                );
            }

            if (IsStringMethod(expression.Method, "StartsWith"))
            {
                return SqlQuery.Concat(
                    ParseExpression(ref i, expression.Object),
                    "LIKE",
                    ParseExpression(ref i, expression.Arguments[0], suffix: "%")
                );
            }

            if (IsStringMethod(expression.Method, "EndsWith"))
            {
                return SqlQuery.Concat(
                    ParseExpression(ref i, expression.Object),
                    "LIKE",
                    ParseExpression(ref i, expression.Arguments[0], prefix: "%")
                );
            }

            if (IsStringMethod(expression.Method, "Equals"))
            {
                return SqlQuery.Concat(
                    ParseExpression(ref i, expression.Object),
                    "=",
                    ParseExpression(ref i, expression.Arguments[0], left: false)
                );
            }

            if (expression.Method.Name == "Contains")
            {
                Expression collection;
                Expression property;
                if (expression.Method.IsDefined(typeof(ExtensionAttribute)) && expression.Arguments.Count == 2)
                {
                    collection = expression.Arguments[0];
                    property = expression.Arguments[1];
                }
                else if (!expression.Method.IsDefined(typeof(ExtensionAttribute)) && expression.Arguments.Count == 1)
                {
                    collection = expression.Object;
                    property = expression.Arguments[0];
                }
                else
                {
                    throw new Exception("Unsupported method call: " + expression.Method.Name);
                }

                var values = (IEnumerable)GetValue(collection);

                return SqlQuery.Concat(
                    ParseExpression(ref i, property),
                    "IN",
                    SqlQuery.IsCollection(ref i, values)
                );
            }

            throw new Exception("Unsupported method call: " + expression.Method.Name);
        }

        private SqlQuery MemberExpressionExtract(ref int i, MemberExpression expression, bool isUnary,
            string prefix, string postfix, bool left)
        {
            if (isUnary && expression.Type == typeof(bool))
            {
                return SqlQuery.Concat(ParseExpression(ref i, expression), "=", SqlQuery.IsSql("1"));
            }

            if (expression.Member is PropertyInfo property)
            {
                var colName = SqlGeneratorCache.GetRequiredPropertyMetadata(property).ColumnName;

                if (left)
                {
                    return SqlQuery.IsSql($"{AddBrackets(TableName)}.{AddBrackets(colName)}");
                }

                if (property.PropertyType == typeof(bool))
                {
                    return SqlQuery.IsSql($"{AddBrackets(TableName)}.{AddBrackets(colName)} = 1");
                }
            }

            if (expression.Member is FieldInfo || left == false)
            {
                var value = GetValue(expression);
                return SqlQuery.IsParameter(i++, value is string textValue ? prefix + textValue + postfix : value);
            }

            throw new Exception($"Expression does not refer to a property or field: {expression}");
        }

        private SqlQuery ConstantExpressionExtract(ref int i, ConstantExpression expression, bool isUnary,
            string prefix, string postfix, bool left)
        {
            var value = expression.Value;

            return value switch
            {
                null => SqlQuery.IsSql("NULL"),
                int intValue => SqlQuery.IsSql(intValue.ToString()),
                string text => SqlQuery.IsParameter(i++, prefix + text + postfix),
                bool boolValue when !isUnary => HandleBooleanConstant(boolValue, left),
                _ => SqlQuery.IsParameter(i++, value)
            };
        }

        private SqlQuery HandleBooleanConstant(bool value, bool left)
        {
            return SqlQuery.IsSql(left
                ? (value ? "1 = 1" : "0 = 1")
                : (value ? "1" : "0"));
        }

        private SqlQuery BinaryExpressionExtract(ref int i, BinaryExpression expression)
        {
            return SqlQuery.Concat(
                ParseExpression(ref i, expression.Left),
                NodeTypeToString(expression.NodeType),
                ParseExpression(ref i, expression.Right, left: false)
            );
        }

        private SqlQuery UnaryExpressionExtract(ref int i, UnaryExpression expression)
        {
            return SqlQuery.Concat(
                NodeTypeToString(expression.NodeType),
                ParseExpression(ref i, expression.Operand, true)
            );
        }

        private object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        private string NodeTypeToString(ExpressionType nodeType)
        {
            return _nodeTypeMappings.TryGetValue(nodeType, out var value)
                ? value
                : string.Empty;
        }

        private static string GetPropertyName<TSource>(Expression<Func<TSource, object>> column)
        {
            var expression = UnwrapMemberExpression(column.Body);
            return expression.Member is PropertyInfo property
                ? SqlGeneratorCache.GetRequiredPropertyMetadata(property).ColumnName
                : expression.Member.Name;
        }

        private static MemberExpression UnwrapMemberExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            if (expression is UnaryExpression unaryExpression
                && unaryExpression.NodeType == ExpressionType.Convert)
            {
                return UnwrapMemberExpression(unaryExpression.Operand);
            }

            throw new InvalidCastException($"Unable to resolve member access from expression type.");
        }
    }
}
