using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.SqlGenerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                var precisionAttribute = property.GetCustomAttribute<DbPrecision>();
                var defaultAttribute = property.GetCustomAttribute<DbDefault>();
                var propMetadata = new SqlPropertyMetadata
                {
                    Property = property,
                    ColumnName = property.GetCustomAttribute<DbColumn>()?.Name ?? property.Name,
                    IsIgnored = property.IsDefined(typeof(DbIgnore), true),
                    IsJoin = joinAttribute != null,
                    IsIdentity = property.IsDefined(typeof(DbIdentity), true),
                    MaxLength = property.GetCustomAttribute<DbMaxLength>()?.Max,
                    Precision = precisionAttribute?.Precision,
                    Scale = precisionAttribute?.Scale,
                    DefaultValue = defaultAttribute?.Value
                };

                ValidatePropertyAttributes(property, propMetadata);

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

        private static void ValidatePropertyAttributes(PropertyInfo property, SqlPropertyMetadata metadata)
        {
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (metadata.MaxLength.HasValue)
            {
                if (propertyType != typeof(string))
                {
                    throw new InvalidOperationException($"DbMaxLength can only be used on string properties. '{property.DeclaringType?.FullName}.{property.Name}' is '{propertyType.Name}'.");
                }

                if (metadata.MaxLength.Value <= 0)
                {
                    throw new InvalidOperationException($"DbMaxLength must be greater than zero for '{property.DeclaringType?.FullName}.{property.Name}'.");
                }
            }

            if (metadata.Precision.HasValue || metadata.Scale.HasValue)
            {
                if (propertyType != typeof(decimal))
                {
                    throw new InvalidOperationException($"DbPrecision can only be used on decimal properties. '{property.DeclaringType?.FullName}.{property.Name}' is '{propertyType.Name}'.");
                }

                if (!metadata.Precision.HasValue || !metadata.Scale.HasValue)
                {
                    throw new InvalidOperationException($"DbPrecision requires both precision and scale for '{property.DeclaringType?.FullName}.{property.Name}'.");
                }

                if (metadata.Precision.Value <= 0)
                {
                    throw new InvalidOperationException($"DbPrecision precision must be greater than zero for '{property.DeclaringType?.FullName}.{property.Name}'.");
                }

                if (metadata.Scale.Value < 0 || metadata.Scale.Value > metadata.Precision.Value)
                {
                    throw new InvalidOperationException($"DbPrecision scale must be between 0 and precision for '{property.DeclaringType?.FullName}.{property.Name}'.");
                }
            }

            if (metadata.DefaultValue != null)
            {
                ValidateDefaultAttribute(property, propertyType, metadata.DefaultValue);
            }
        }

        private static void ValidateDefaultAttribute(PropertyInfo property, Type propertyType, string defaultValueLiteral)
        {
            try
            {
                if (propertyType == typeof(string))
                {
                    return;
                }

                if (propertyType == typeof(bool))
                {
                    _ = bool.Parse(defaultValueLiteral);
                    return;
                }

                if (propertyType == typeof(byte))
                {
                    _ = byte.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(short))
                {
                    _ = short.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(int))
                {
                    _ = int.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(long))
                {
                    _ = long.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(float))
                {
                    _ = float.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(double))
                {
                    _ = double.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(decimal))
                {
                    _ = decimal.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                if (propertyType == typeof(Guid))
                {
                    _ = Guid.Parse(defaultValueLiteral);
                    return;
                }

                if (propertyType == typeof(DateTime))
                {
                    _ = DateTime.Parse(defaultValueLiteral, CultureInfo.InvariantCulture);
                    return;
                }

                throw new InvalidOperationException($"Default is not supported for property type '{propertyType.Name}' on {property.DeclaringType?.FullName}.{property.Name}.");
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentException)
            {
                throw new InvalidOperationException($"Default value '{defaultValueLiteral}' is invalid for {property.DeclaringType?.FullName}.{property.Name}.", ex);
            }
        }
    }
}
