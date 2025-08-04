using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.SqlGenerator.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        private static readonly ConcurrentDictionary<Type, SqlMetadata> _tableCache = new ConcurrentDictionary<Type, SqlMetadata>();

        public DatabaseType DatabaseType { get; protected set; }
        public string TableDatabase { get; protected set; }
        public string TableSchema { get; protected set; }
        public string TableName { get; protected set; }
        public List<PropertyInfo> AllProperties { get; protected set; } = new List<PropertyInfo>();

        public List<PropertyInfo> Properties { get; protected set; } = new List<PropertyInfo>();
        public List<PropertyInfo> IgnoreProperties { get; protected set; } = new List<PropertyInfo>();
        public List<PropertyInfo> JoinProperties { get; protected set; } = new List<PropertyInfo>();
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

            if (_tableCache.TryGetValue(type, out SqlMetadata metadata))
            {
                TableDatabase = metadata.TableDatabase;
                TableSchema = metadata.TableSchema;
                TableName = metadata.TableName;

                AllProperties = metadata.AllProperties;
                Properties = metadata.Properties;
                IgnoreProperties = metadata.IgnoreProperties;
                JoinProperties = metadata.JoinProperties;


                FullTableNameSqlServer = metadata.FullTableNameSqlServer;
                FullTableNameMySql = metadata.FullTableNameMySql;

                return;
            }

            var dbTable = type.GetCustomAttribute<DbTable>()
                ?? throw new Exception("Entity must have a DbTable attribute");

            TableDatabase = dbTable.Database ?? string.Empty;
            TableSchema = dbTable.Schema ?? _defaultSchema;
            TableName = dbTable.Name;

            FullTableNameSqlServer = FormatFullTableName(TableDatabase, TableSchema, TableName, DatabaseType.SqlServer);
            FullTableNameMySql = FormatFullTableName(TableDatabase, string.Empty, TableName, DatabaseType.MySql);

            AllProperties = type.GetProperties().ToList();

            var dbIgnore = typeof(DbIgnore);
            var dbJoin = typeof(DBJoin);

            foreach (var prop in AllProperties)
            {
                bool isIgnore = prop.IsDefined(dbIgnore, true);
                bool isJoin = prop.IsDefined(dbJoin, true);

                if (isIgnore) IgnoreProperties.Add(prop);
                else if (isJoin) JoinProperties.Add(prop);
                else Properties.Add(prop);
            }

            _tableCache.TryAdd(type, new SqlMetadata
            {
                TableDatabase = TableDatabase,
                TableSchema = TableSchema,
                TableName = TableName,

                AllProperties = AllProperties,
                Properties = Properties,
                IgnoreProperties = IgnoreProperties,
                JoinProperties = JoinProperties,

                FullTableNameSqlServer = FullTableNameSqlServer,
                FullTableNameMySql = FullTableNameMySql
            });
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
            var success = _tableCache.TryGetValue(dbTable.GetType(), out SqlMetadata metaData);

            if (success)
            {
                return DatabaseType switch
                {
                    DatabaseType.SqlServer => metaData.FullTableNameSqlServer,
                    DatabaseType.MySql => metaData.FullTableNameMySql,
                    _ => FormatFullTableName(TableDatabase, TableSchema, TableName, DatabaseType)
                };
            }

            return FormatFullTableName(dbTable.Database, dbTable.Schema, dbTable.Name, DatabaseType);
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
