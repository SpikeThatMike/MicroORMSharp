using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public SqlQuery UpdateRow(T obj, bool returnValue = false)
        {
            return UpdateRow(obj, DataProperties, returnValue);
        }

        public SqlQuery UpdateRow(T obj, Expression<Func<T, object>> columns, bool returnValue = false)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            var selectedProperties = ResolveUpdateProperties(columns);
            return UpdateRow(obj, selectedProperties, returnValue);
        }

        private SqlQuery UpdateRow(T obj, IEnumerable<PropertyInfo> updateProperties, bool returnValue)
        {
            var updatePropertyList = updateProperties?.Distinct().ToList()
                ?? throw new ArgumentNullException(nameof(updateProperties));

            if (!updatePropertyList.Any())
            {
                throw new InvalidOperationException("At least one updatable property must be selected.");
            }

            ValidateAttributes(obj, updatePropertyList);

            var newQuery = new SqlQuery();

            var identityProps = IdentityProperties;

            if (identityProps == null || !identityProps.Any())
                throw new InvalidOperationException("No identity column found.");

            var whereClause = new List<string>();

            int count = 0;
            foreach (var prop in identityProps)
            {
                var parameter = $"@p{++count}";
                whereClause.Add($"{AddBrackets(TableName)}.{AddBrackets(GetPropertyName(prop))} = {parameter}");
                newQuery.Parameters.Add(parameter, prop.GetValue(obj));
            }

            var updateColumns = new List<string>();

            foreach (var prop in updatePropertyList)
            {
                if (ShouldUseDefaultValue(prop, obj))
                {
                    updateColumns.Add($"{AddBrackets(TableName)}.{AddBrackets(GetPropertyName(prop))} = DEFAULT");
                    continue;
                }

                var parameter = $"@p{++count}";

                updateColumns.Add($"{AddBrackets(TableName)}.{AddBrackets(GetPropertyName(prop))} = {parameter}");
                newQuery.Parameters.Add(parameter, prop.GetValue(obj));
            }

            newQuery.Query.Append($"UPDATE {GetFullTableName()} SET {string.Join(", ", updateColumns)} ");
            newQuery.Query.Append($"WHERE {string.Join(" AND ", whereClause)};");

            if (returnValue)
            {
                var selectColumns = Properties.Select(x => (MemberInfo)x);
                newQuery.Query.Append($" SELECT {string.Join(", ", GenerateSelectClause(TableName, selectColumns))} FROM {GetFullTableName()} WHERE {string.Join(" AND ", whereClause)};");
            }

            return newQuery;
        }

        private List<PropertyInfo> ResolveUpdateProperties(Expression<Func<T, object>> columns)
        {
            var selectedProperties = ExtractSelectedProperties(columns.Body)
                .Distinct()
                .ToList();

            if (!selectedProperties.Any())
            {
                throw new InvalidOperationException("At least one property must be selected for update.");
            }

            var invalidProperties = selectedProperties
                .Where(x => !DataProperties.Contains(x))
                .ToList();

            if (invalidProperties.Any())
            {
                throw new InvalidOperationException(
                    $"Only mapped, non-identity properties can be updated. Invalid selections: {string.Join(", ", invalidProperties.Select(x => x.Name))}."
                );
            }

            return selectedProperties;
        }

        private IEnumerable<PropertyInfo> ExtractSelectedProperties(Expression expression)
        {
            switch (expression)
            {
                case MemberExpression memberExpression when memberExpression.Member is PropertyInfo propertyInfo:
                    yield return propertyInfo;
                    yield break;

                case UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert || unaryExpression.NodeType == ExpressionType.ConvertChecked:
                    foreach (var property in ExtractSelectedProperties(unaryExpression.Operand))
                    {
                        yield return property;
                    }
                    yield break;

                case NewExpression newExpression:
                    foreach (var argument in newExpression.Arguments)
                    {
                        foreach (var property in ExtractSelectedProperties(argument))
                        {
                            yield return property;
                        }
                    }
                    yield break;

                case MemberInitExpression memberInitExpression:
                    foreach (var binding in memberInitExpression.Bindings.OfType<MemberAssignment>())
                    {
                        foreach (var property in ExtractSelectedProperties(binding.Expression))
                        {
                            yield return property;
                        }
                    }
                    yield break;

                default:
                    throw new ArgumentException("Columns expression must select one or more properties, for example x => x.Name or x => new { x.Name, x.Email }.", nameof(expression));
            }
        }
    }
}
