using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public SqlQuery CreateTable()
        {
            var newQuery = new SqlQuery();

            newQuery.Query.Append($"CREATE TABLE {GetFullTableName()} (");

            List<string> props = new List<string>();
            foreach (var prop in Properties)
            {
                props.Add($"{GetPropertyName(prop)} {GetSqlProperty(prop).Trim()}");
            }

            newQuery.Query.Append(string.Join(", ", props));

            newQuery.Query.Append($")");

            return newQuery;
        }

        private string GetSqlProperty(PropertyInfo prop)
        {
            bool isNullable = !prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) != null;
            Type propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            string sqlType = propertyType switch
            {
                Type t when t == typeof(string) => $"VARCHAR({GetDBMaxLength(prop)})",
                Type t when t == typeof(byte) => "TINYINT",
                Type t when t == typeof(short) => "SMALLINT",
                Type t when t == typeof(int) => "INT",
                Type t when t == typeof(long) => "BIGINT",
                Type t when t == typeof(double) => "FLOAT",
                Type t when t == typeof(float) => "REAL",
                Type t when t == typeof(decimal) => "DECIMAL(18, 2)",
                Type t when t == typeof(bool) => "BIT",
                Type t when t == typeof(DateTime) => "DATETIME",
                Type t when t == typeof(Guid) => "UNIQUEIDENTIFIER",
                Type t when t == typeof(byte[]) => $"VARBINARY({GetDBMaxLength(prop)})",
                _ => throw new NotSupportedException($"The C# type '{prop.Name}' is not supported."),
            };

            if (DatabaseType == DatabaseType.MySql && sqlType == "VARCHAR(MAX)")
            {
                sqlType = "LONGTEXT";
            }
            else if (DatabaseType == DatabaseType.MySql && sqlType == "VARBINARY(MAX)")
            {
                sqlType = "LONGBLOB";
            }

            return (isNullable ? sqlType : $"{sqlType} NOT NULL") + GetAdditionalProperties(prop);
        }

        private string GetDBMaxLength(PropertyInfo prop)
        {
            return GetPropertyMetadata(prop).MaxLength?.ToString() ?? "MAX";
        }

        private string GetAdditionalProperties(PropertyInfo prop)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (GetPropertyMetadata(prop).IsIdentity)
            {
                if (DatabaseType == DatabaseType.SqlServer)
                {
                    stringBuilder.Append(" IDENTITY(1,1)");
                }
                else if (DatabaseType == DatabaseType.MySql)
                {
                    stringBuilder.Append(" AUTO_INCREMENT PRIMARY KEY");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
