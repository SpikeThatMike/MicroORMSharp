using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

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

            //SQL SERVER TOP
            if (DatabaseType == DatabaseType.SqlServer && dbQuery._take != null)
            {
                sqlQuery.Query.Append($" TOP ({dbQuery._take}) ");
            }

            //Select rows
            var selectColumns = dbQuery._selectClause != null && dbQuery._selectClause.Any() ? dbQuery._selectClause : Properties.Select(x => (MemberInfo)x);
            sqlQuery.Query.Append($" {string.Join(", ", GenerateSelectClause(selectColumns))} FROM {GetFullTableName()}");

            //Where clause
            if (dbQuery._whereClause != null)
            {
                var filter = GenerateWhereClause(dbQuery._whereClause.Body);
                sqlQuery.Parameters = filter.Parameters;
                sqlQuery.Query.Append($" WHERE {filter.ToString()}");
            }

            //Order by
            if (dbQuery._orderBy.Any())
            {
                sqlQuery.Query.Append(" ORDER BY ");
                sqlQuery.Query.Append(string.Join(", ", dbQuery._orderBy.Select(x => $"{AddBrackets(x.Key)} {(x.Value ? "DESC" : "ASC")}")));
            }

            //MY SQL LIMIT
            if (DatabaseType == DatabaseType.MySql && dbQuery._take != null)
            {
                sqlQuery.Query.Append($" LIMIT {dbQuery._take}");
            }

            return sqlQuery;
        }

        private IEnumerable<string> GenerateSelectClause(IEnumerable<MemberInfo> memberInfo)
        {
            List<string> columns = new List<string>();

            foreach (var member in memberInfo)
            {
                var classColumn = member.Name;
                var dbColumn = member.GetCustomAttribute<DbColumn>()?.Name ?? classColumn;

                columns.Add($"{AddBrackets(dbColumn)} AS {AddBrackets(classColumn)}");
            }

            return columns;
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
            #region SQL Like Methods - String
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
            #endregion


            #region SQL Contains Method - Lists
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
            #endregion

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
                var colName = property.GetCustomAttribute<Attributes.DbColumn>()?.Name ?? property.Name;
                var tableName = property.DeclaringType.GetCustomAttribute<Attributes.DbTable>()?.Name;

                if (left)
                {
                    return SqlQuery.IsSql($"{AddBrackets(tableName)}.{AddBrackets(colName)}");
                }

                if (property.PropertyType == typeof(bool))
                {
                    return SqlQuery.IsSql($"{AddBrackets(tableName)}.{AddBrackets(colName)} = 1");
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
    }
}
