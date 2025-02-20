using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public SqlQuery UpdateRow(T obj, bool returnValue = false)
        {
            var newQuery = new SqlQuery();

            var identityProps = Properties.Where(prop => prop.GetCustomAttribute<DbIdentity>() != null);

            if (identityProps == null || !identityProps.Any())
                throw new InvalidOperationException("No identity column found.");

            var whereClause = new List<string>();

            int count = 0;
            foreach (var prop in identityProps)
            {
                var parameter = $"@p{++count}";
                whereClause.Add($"{GetPropertyName(prop)} = {parameter}");
                newQuery.Parameters.Add(parameter, prop.GetValue(obj));
            }

            var updateColumns = new List<string>();

            foreach (var prop in Properties.Where(OnlyDataColumns))
            {
                var parameter = $"@p{++count}";

                updateColumns.Add($"{GetPropertyName(prop)} = {parameter}");
                newQuery.Parameters.Add(parameter, prop.GetValue(obj));
            }

            newQuery.Query.Append($"UPDATE {GetFullTableName()} SET {string.Join(", ", updateColumns)} ");
            newQuery.Query.Append($"WHERE {string.Join(" AND ", whereClause)};");

            if (returnValue)
            {
                var selectColumns = Properties.Select(x => (MemberInfo)x);
                newQuery.Query.Append($" SELECT {string.Join(", ", GenerateSelectClause(selectColumns))} FROM {GetFullTableName()} WHERE {string.Join(" AND ", whereClause)};");
            }

            return newQuery;
        }
    }
}
