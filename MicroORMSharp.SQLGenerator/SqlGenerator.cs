using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.SqlGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public DatabaseType DatabaseType { get; protected set; }
        public string TableDatabase { get; protected set; }
        public string TableSchema { get; protected set; }
        public string TableName { get; protected set; }
        public List<PropertyInfo> AllProperties { get; protected set; } = new List<PropertyInfo>();
        public List<PropertyInfo> Properties { get; protected set; } = new List<PropertyInfo>();
        public List<PropertyInfo> IgnoreProperties { get; protected set; } = new List<PropertyInfo>();
        public List<PropertyInfo> JoinProperties { get; protected set; } = new List<PropertyInfo>();
        private List<PropertyInfo> IdentityProperties { get; set; } = new List<PropertyInfo>();
        private List<PropertyInfo> DataProperties { get; set; } = new List<PropertyInfo>();
        private Dictionary<PropertyInfo, SqlPropertyMetadata> PropertyMetadata { get; set; } = new Dictionary<PropertyInfo, SqlPropertyMetadata>();
        private string _defaultSchema { get; set; } = "dbo";

        public string FullTableNameSqlServer { get; set; }
        public string FullTableNameMySql { get; set; }

        public SqlGenerator(DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            Init();
        }

        public SqlGenerator(DatabaseType databaseType, string defaultSchema)
        {
            DatabaseType = databaseType;
            _defaultSchema = defaultSchema;
            Init();
        }

        private void Init()
        {
            Type? type = typeof(T)
                ?? throw new Exception($"{nameof(type)} Type cannot be null");

            var metadata = SqlGeneratorCache.GetRequiredMetadata(type);

            TableDatabase = metadata.TableDatabase ?? string.Empty;
            TableSchema = metadata.TableSchema ?? _defaultSchema;
            TableName = metadata.TableName;
            AllProperties = metadata.AllProperties;
            Properties = metadata.Properties;
            IgnoreProperties = metadata.IgnoreProperties;
            JoinProperties = metadata.JoinProperties;
            IdentityProperties = metadata.IdentityProperties;
            DataProperties = metadata.DataProperties;
            PropertyMetadata = metadata.PropertyMetadata;

            FullTableNameSqlServer = FormatFullTableName(TableDatabase, TableSchema, TableName, DatabaseType.SqlServer);
            FullTableNameMySql = FormatFullTableName(TableDatabase, string.Empty, TableName, DatabaseType.MySql);
        }

        public string GetFullTableName()
        {
            return DatabaseType switch
            {
                DatabaseType.SqlServer => FullTableNameSqlServer,
                DatabaseType.MySql => FullTableNameMySql,
                _ => FormatFullTableName(TableDatabase, TableSchema, TableName, DatabaseType)
            };
        }

        public string GetFullTableName(DbTable dbTable)
        {
            return FormatFullTableName(dbTable.Database, dbTable.Schema, dbTable.Name, DatabaseType);
        }

        private SqlMetadata GetMetadata(Type type)
        {
            return SqlGeneratorCache.GetRequiredMetadata(type);
        }

        private SqlPropertyMetadata GetPropertyMetadata(PropertyInfo property)
        {
            return PropertyMetadata.TryGetValue(property, out var metadata)
                ? metadata
                : SqlGeneratorCache.GetRequiredPropertyMetadata(property);
        }

        private string GetPropertyName(PropertyInfo prop)
        {
            return GetPropertyMetadata(prop).ColumnName;
        }

        private string GetPropertyName(MemberInfo prop)
        {
            if (prop is PropertyInfo property)
            {
                return GetPropertyName(property);
            }

            return prop.Name;
        }

        private string AddBrackets(string identifier, DatabaseType? databaseType = null)
        {
            databaseType ??= DatabaseType;

            if (databaseType == DatabaseType.SqlServer)
                return $"[{identifier}]";
            else if (databaseType == DatabaseType.MySql)
                return $"`{identifier}`";

            return identifier;
        }

        private string FormatFullTableName(string database, string schema, string table, DatabaseType dbType)
        {
            var parts = new List<string>
            {
                database,
                dbType == DatabaseType.SqlServer ? schema : string.Empty,
                table
            }.Where(x => !string.IsNullOrEmpty(x));

            return string.Join(".", parts.Select(x => AddBrackets(x, dbType)));
        }
    }
}
