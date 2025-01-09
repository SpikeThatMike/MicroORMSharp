using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _propertyCache = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        public DatabaseType DatabaseType { get; protected set; }
        public string TableDatabase { get; protected set; }
        public string TableSchema { get; protected set; }
        public string TableName { get; protected set; }
        public IEnumerable<PropertyInfo> AllProperties { get; protected set; } = new List<PropertyInfo>();

        public IEnumerable<PropertyInfo> Properties { get; protected set; } = new List<PropertyInfo>();
        public IEnumerable<PropertyInfo> IgnoreProperties { get; protected set; } = new List<PropertyInfo>();

        private string _defaultSchema { get; set; } = "dbo";

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
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            var dbTable = typeInfo.GetCustomAttribute<DbTable>();

            if (dbTable == null)
            {
                throw new Exception("Entity must have a DbTable attribute");
            }

            TableDatabase = dbTable.Database ?? "";
            TableSchema = dbTable.Schema ?? _defaultSchema;
            TableName = dbTable.Name; //Cannot be null

            if (_propertyCache.TryGetValue(type, out IEnumerable<PropertyInfo> properties))
            {
                AllProperties = properties.ToList();
            }
            else
            {
                AllProperties = type.GetProperties();
                _propertyCache.TryAdd(type, AllProperties);
            }

            Properties = AllProperties.Where(x => x.GetCustomAttribute<DbIgnore>() == null);
            IgnoreProperties = AllProperties.Where(x => x.GetCustomAttribute<DbIgnore>() != null);
        }

        public string GetFullTableName()
        {
            IEnumerable<string> sb = new List<string>()
            {
                TableDatabase,
                DatabaseType == DatabaseType.SqlServer ? TableSchema : null,
                TableName
            }.Where(x => !string.IsNullOrEmpty(x));

            return string.Join(".", sb.Select(AddBrackets));
        }

        private string AddBrackets(string identifier)
        {
            if (DatabaseType == DatabaseType.SqlServer)
                return $"[{identifier}]";
            else if (DatabaseType == DatabaseType.MySql)
                return $"`{identifier}`";

            return identifier;
        }
    }
}
