using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                Type t when t == typeof(decimal) => GetDecimalSqlType(prop),
                Type t when t == typeof(bool) => "BIT",
                Type t when t == typeof(DateTime) => "DATETIME",
                Type t when t == typeof(Guid) => "UNIQUEIDENTIFIER",
                Type t when t == typeof(byte[]) => "VARBINARY(MAX)",
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

        private string GetDecimalSqlType(PropertyInfo prop)
        {
            var metadata = GetPropertyMetadata(prop);
            var precision = metadata.Precision ?? 18;
            var scale = metadata.Scale ?? 2;

            return $"DECIMAL({precision}, {scale})";
        }

        private string GetAdditionalProperties(PropertyInfo prop)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var metadata = GetPropertyMetadata(prop);

            if (metadata.IsIdentity)
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

            if (metadata.DefaultValue != null)
            {
                stringBuilder.Append($" DEFAULT {GetSqlDefaultLiteral(prop, metadata.DefaultValue)}");
            }

            return stringBuilder.ToString();
        }

        private string GetSqlDefaultLiteral(PropertyInfo prop, string defaultValueLiteral)
        {
            var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (propertyType == typeof(string) || propertyType == typeof(Guid) || propertyType == typeof(DateTime))
            {
                return $"'{defaultValueLiteral.Replace("'", "''")}'";
            }

            if (propertyType == typeof(bool))
            {
                return bool.Parse(defaultValueLiteral) ? "1" : "0";
            }

            if (propertyType == typeof(byte) || propertyType == typeof(short) || propertyType == typeof(int) || propertyType == typeof(long))
            {
                return Convert.ToString(Convert.ChangeType(defaultValueLiteral, propertyType, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            }

            if (propertyType == typeof(float))
            {
                return float.Parse(defaultValueLiteral, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            }

            if (propertyType == typeof(double))
            {
                return double.Parse(defaultValueLiteral, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            }

            if (propertyType == typeof(decimal))
            {
                return decimal.Parse(defaultValueLiteral, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException($"Default is not supported for property {prop.DeclaringType?.FullName}.{prop.Name}.");
        }
    }
}
