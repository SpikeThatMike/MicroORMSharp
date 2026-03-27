using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.SqlGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator
{
    public static class SqlGeneratorCache
    {
        private static Dictionary<Type, SqlMetadata> _metadataByType = new Dictionary<Type, SqlMetadata>();
        private static bool _isInitialized;

        public static void Initialise()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            var interfaceType = typeof(IMicroORMSharp);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && interfaceType.IsAssignableFrom(x));

            var metadata = new Dictionary<Type, SqlMetadata>();

            foreach (var type in types)
            {
                BuildMetadata(type, metadata);
            }

            _metadataByType = metadata;
        }

        internal static SqlMetadata GetRequiredMetadata(Type type)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("SqlGeneratorCache.Initialise() must be called during startup before using SqlGenerator.");
            }

            if (!_metadataByType.TryGetValue(type, out var metadata))
            {
                throw new InvalidOperationException($"Type '{type.FullName}' is not registered.");
            }

            return metadata;
        }

        internal static SqlPropertyMetadata GetRequiredPropertyMetadata(PropertyInfo property)
        {
            var declaringType = property.DeclaringType
                ?? throw new InvalidOperationException($"Property '{property.Name}' does not have a declaring type.");

            var metadata = GetRequiredMetadata(declaringType);
            if (!metadata.PropertyMetadata.TryGetValue(property, out var propertyMetadata))
            {
                throw new InvalidOperationException($"Property '{declaringType.FullName}.{property.Name}' is not registered in SqlGeneratorCache.");
            }

            return propertyMetadata;
        }

        private static SqlMetadata BuildMetadata(Type type, IDictionary<Type, SqlMetadata> cache)
        {
            if (cache.TryGetValue(type, out var existing))
            {
                return existing;
            }

            var dbTable = type.GetCustomAttribute<DbTable>()
                ?? throw new Exception($"Entity '{type.FullName}' must have a DbTable attribute");

            var allProperties = type.GetProperties().ToList();
            var propertyMetadata = new Dictionary<PropertyInfo, SqlPropertyMetadata>();

            var metadata = new SqlMetadata
            {
                EntityType = type,
                TableDatabase = dbTable.Database ?? string.Empty,
                TableSchema = dbTable.Schema,
                TableName = dbTable.Name,
                AllProperties = allProperties,
                Properties = new List<PropertyInfo>(),
                IgnoreProperties = new List<PropertyInfo>(),
                JoinProperties = new List<PropertyInfo>(),
                IdentityProperties = new List<PropertyInfo>(),
                DataProperties = new List<PropertyInfo>(),
                PropertyMetadata = propertyMetadata
            };

            cache[type] = metadata;

            foreach (var property in allProperties)
            {
                var joinAttribute = property.GetCustomAttribute<DBJoin>();
                var propMetadata = new SqlPropertyMetadata
                {
                    Property = property,
                    ColumnName = property.GetCustomAttribute<DbColumn>()?.Name ?? property.Name,
                    IsIgnored = property.IsDefined(typeof(DbIgnore), true),
                    IsJoin = joinAttribute != null,
                    IsIdentity = property.IsDefined(typeof(DbIdentity), true),
                    MaxLength = property.GetCustomAttribute<DbMaxLength>()?.Max
                };

                if (joinAttribute != null)
                {
                    propMetadata.Join = new SqlJoinMetadata
                    {
                        JoinedType = joinAttribute.Type,
                        TableKey = joinAttribute.TableKey,
                        OtherKey = joinAttribute.OtherKey,
                        JoinType = joinAttribute.JoinType
                    };

                    BuildMetadata(joinAttribute.Type, cache);
                }

                propertyMetadata[property] = propMetadata;

                if (propMetadata.IsIgnored)
                {
                    metadata.IgnoreProperties.Add(property);
                    continue;
                }

                if (propMetadata.IsJoin)
                {
                    metadata.JoinProperties.Add(property);
                    continue;
                }

                metadata.Properties.Add(property);

                if (propMetadata.IsIdentity)
                {
                    metadata.IdentityProperties.Add(property);
                }
                else
                {
                    metadata.DataProperties.Add(property);
                }
            }

            return metadata;
        }
    }
}
