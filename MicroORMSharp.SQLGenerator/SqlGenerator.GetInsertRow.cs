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
        public SqlQuery InsertRow(T obj, bool returnValue = true)
        {
            var newQuery = new SqlQuery();

            var columnNames = new List<string>();

            int count = 0;
            foreach (var prop in Properties.Where(OnlyDataColumns))
            {
                columnNames.Add(GetPropertyName(prop));
                newQuery.Parameters.Add($"@p{++count}", prop.GetValue(obj));
            }

            newQuery.Query.Append($"INSERT INTO {GetFullTableName()} ({string.Join(", ", columnNames)}) ");
            newQuery.Query.Append($"VALUES ({string.Join(", ", newQuery.Parameters.Select(x => x.Key))});");
            if (returnValue)
            {
                var selectColumns = Properties.Select(x => (MemberInfo)x);
                newQuery.Query.Append($" SELECT {string.Join(", ", GenerateSelectClause(selectColumns))} FROM {GetFullTableName()} WHERE Id = (SELECT {GetLastInsertMethod()});");
            }

            return newQuery;
        }

        private string GetLastInsertMethod() => DatabaseType switch
        {
            DatabaseType.MySql => "LAST_INSERT_ID()",
            DatabaseType.SqlServer => "SCOPE_IDENTITY()",
            _ => throw new Exception($"Unknown database type: {DatabaseType}")
        };


        private bool OnlyDataColumns(PropertyInfo prop)
        {
            if (prop.GetCustomAttribute<DbIdentity>() != null)
                return false;

            return true;
        }
    }
}
