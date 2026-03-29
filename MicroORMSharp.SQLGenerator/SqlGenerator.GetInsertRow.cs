using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public SqlQuery InsertRow(T obj, bool returnValue = true)
        {
            ValidateAttributes(obj);

            var newQuery = new SqlQuery();

            var columnNames = new List<string>();
            var values = new List<string>();

            int count = 0;
            foreach (var prop in DataProperties)
            {
                columnNames.Add(GetPropertyName(prop));

                if (ShouldUseDefaultValue(prop, obj))
                {
                    values.Add("DEFAULT");
                    continue;
                }

                var parameterName = $"@p{++count}";
                values.Add(parameterName);
                newQuery.Parameters.Add(parameterName, prop.GetValue(obj));
            }

            var identityProp = IdentityProperties.FirstOrDefault()
                ?? throw new Exception("No identity column found. Please ensure that one property is marked with the DbIdentity attribute.");

            newQuery.Query.Append($"INSERT INTO {GetFullTableName()} ({string.Join(", ", columnNames)}) ");
            newQuery.Query.Append($"VALUES ({string.Join(", ", values)});");
            if (returnValue)
            {
                var selectColumns = Properties.Select(x => (MemberInfo)x);
                newQuery.Query.Append($" SELECT {string.Join(", ", GenerateSelectClause(TableName, selectColumns))} FROM {GetFullTableName()} WHERE {AddBrackets(TableName)}.{AddBrackets(GetPropertyName(identityProp))} = (SELECT {GetLastInsertMethod()});");
            }

            return newQuery;
        }

        private string GetLastInsertMethod() => DatabaseType switch
        {
            DatabaseType.MySql => "LAST_INSERT_ID()",
            DatabaseType.SqlServer => "SCOPE_IDENTITY()",
            _ => throw new Exception($"Unknown database type: {DatabaseType}")
        };

        private bool ShouldUseDefaultValue(PropertyInfo prop, T obj)
        {
            var metadata = GetPropertyMetadata(prop);
            return metadata.DefaultValue != null && prop.GetValue(obj) == null;
        }
    }
}
