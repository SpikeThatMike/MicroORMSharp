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
        public SqlQuery DeleteRow(T obj)
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

            newQuery.Query.Append($"DELETE FROM {GetFullTableName()} WHERE {string.Join(" AND ", whereClause)}");

            return newQuery;
        }
    }
}
