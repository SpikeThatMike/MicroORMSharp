using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        public static IEnumerable<T> Execute<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            if (sqlGenerator.JoinProperties.Any())
            {
                return ExecuteJoin(dbQuery, sqlGenerator);
            }

            var sqlQuery = sqlGenerator.Select(dbQuery);
            return WithQueryConnection(db =>
            {
                return db.Query<T>(
                    new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    transaction: dbQuery._dbTransaction,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }, dbQuery);
        }

        public static async Task<IEnumerable<T>> ExecuteAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());

            if (sqlGenerator.JoinProperties.Any())
            {
                return await ExecuteJoinAsync(dbQuery, sqlGenerator);
            }

            var sqlQuery = sqlGenerator.Select(dbQuery);
            return await WithQueryConnectionAsync(async db =>
            {
                return await db.QueryAsync<T>(
                    new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    transaction: dbQuery._dbTransaction,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }, dbQuery);
        }

        public static IEnumerable<Result> Execute<T, Result>(this DbProjectionQuery<T, Result> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            var selector = dbQuery.Selector.Compile();
            return dbQuery.Query.Execute().Select(selector).ToList();
        }

        public static async Task<IEnumerable<Result>> ExecuteAsync<T, Result>(this DbProjectionQuery<T, Result> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            var selector = dbQuery.Selector.Compile();
            return (await dbQuery.Query.ExecuteAsync()).Select(selector).ToList();
        }

        private static IEnumerable<T> ExecuteJoin<T>(this DbQuery<T> dbQuery, SqlGenerator<T> sqlGenerator) where T : IMicroORMSharp
        {
            var sqlQuery = sqlGenerator.Select(dbQuery);

            return WithQueryConnection(db =>
            {
                var rows = db.Query<dynamic>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    transaction: dbQuery._dbTransaction,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));

                return MapJoinedRows(rows, sqlGenerator);
            }, dbQuery);
        }

        private static async Task<IEnumerable<T>> ExecuteJoinAsync<T>(this DbQuery<T> dbQuery, SqlGenerator<T> sqlGenerator) where T : IMicroORMSharp
        {
            var sqlQuery = sqlGenerator.Select(dbQuery);

            return await WithQueryConnectionAsync(async db =>
            {
                var rows = await db.QueryAsync<dynamic>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    transaction: dbQuery._dbTransaction,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
                return MapJoinedRows(rows, sqlGenerator);
            }, dbQuery);
        }

        private static TResult WithQueryConnection<T, TResult>(Func<IDbConnection, TResult> action, DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            var existingConnection = dbQuery._dbConnection ?? dbQuery._dbTransaction?.Connection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            return WithConnection(action);
        }

        private static async Task<TResult> WithQueryConnectionAsync<T, TResult>(Func<IDbConnection, Task<TResult>> action, DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            var existingConnection = dbQuery._dbConnection ?? dbQuery._dbTransaction?.Connection;
            if (existingConnection != null)
            {
                return await action(existingConnection);
            }

            return await WithConnectionAsync(action);
        }

        private static List<T> MapJoinedRows<T>(IEnumerable<dynamic> rows, SqlGenerator<T> sqlGenerator) where T : IMicroORMSharp
        {
            var results = new List<Dictionary<string, Dictionary<string, object>>>();
            var tableSegments = GetTableSegments(typeof(T), sqlGenerator.TableName, 1).ToList();

            foreach (var row in rows)
            {
                var rowValues = ((IDictionary<string, object>)row).ToList();
                var tables = new Dictionary<string, Dictionary<string, object>>();
                int offset = 0;

                foreach (var tableSegment in tableSegments)
                {
                    var tableValues = new Dictionary<string, object>();

                    foreach (var property in tableSegment.Properties)
                    {
                        object value = offset < rowValues.Count ? rowValues[offset].Value : null;
                        tableValues[property.Name] = value;
                        offset++;
                    }

                    tables[tableSegment.TableName] = tableValues;
                }

                results.Add(tables);
            }

            return MapResultsWithJoins<T>(results);
        }

        private static string GetTableName(Type type)
        {
            return type.GetCustomAttribute<DbTable>()?.Name ?? type.Name;
        }

        public static List<T> MapResultsWithJoins<T>(List<Dictionary<string, Dictionary<string, object>>> results)
        {
            List<T> mappedParents = new List<T>();
            var parentName = typeof(T).GetCustomAttribute<DbTable>()?.Name ?? typeof(T).Name;
            var parentProperties = GetScalarProperties(typeof(T)).ToList();

            foreach (var result in results)
            {
                var parentObj = result[parentName];

                // Check if it exists
                T existingParent = default;
                foreach (var mapped in mappedParents)
                {
                    bool isMatch = true;
                    foreach (var prop in parentProperties)
                    {
                        var resultValue = parentObj[prop.Name];
                        var existingValue = prop.GetValue(mapped);

                        // skip nulls
                        if (resultValue == null || resultValue == DBNull.Value)
                            continue;

                        // compare
                        if (existingValue == null || !Convert.ChangeType(resultValue, prop.PropertyType).Equals(existingValue))
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        existingParent = mapped;
                        break;
                    }
                }

                // If there is no match
                T parent = existingParent;
                if (parent == null)
                {
                    parent = Activator.CreateInstance<T>();

                    foreach (var prop in parentProperties)
                    {
                        var value = parentObj[prop.Name];
                        if (value != null && value != DBNull.Value)
                        {
                            try
                            {
                                prop.SetValue(parent, Convert.ChangeType(value, prop.PropertyType));
                            }
                            catch (InvalidCastException)
                            {
                                // Handle nullable types
                                if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                {
                                    prop.SetValue(parent, Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)));
                                }
                            }
                        }
                    }

                    // Initialize any collection properties for joins
                    CreateJoins(typeof(T), parent, 1);

                    // Add to our list of tracked parents
                    mappedParents.Add(parent);
                }

                // Process joins
                MapJoins(parent, typeof(T), result, 1);
            }

            return mappedParents;
        }

        private static void MapJoins(object parent, Type parentType, Dictionary<string, Dictionary<string, object>> row, int depth)
        {
            EnsureJoinDepth(depth);

            foreach (var joinProperty in GetJoinProperties(parentType))
            {
                var joinAttr = joinProperty.GetCustomAttribute<DBJoin>();
                if (joinAttr == null)
                {
                    continue;
                }

                var childType = GetJoinedEntityType(joinProperty.PropertyType);
                var joinName = GetTableName(childType);

                if (!row.TryGetValue(joinName, out var joinData))
                {
                    continue;
                }

                // data is empty or all null
                if (joinData == null || joinData.All(x => x.Value == null || x.Value == DBNull.Value))
                {
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(joinProperty.PropertyType) && joinProperty.PropertyType != typeof(string))
                {
                    // one-to-many
                    if (!(joinProperty.GetValue(parent) is IList collection))
                    {
                        collection = (IList)CreateJoinListInstance(joinProperty.PropertyType);
                        joinProperty.SetValue(parent, collection);
                    }

                    var childObj = FindExistingEntity(collection, childType, joinData)
                        ?? CreateMappedEntity(childType, joinData);

                    if (!collection.Contains(childObj))
                    {
                        collection.Add(childObj);
                    }

                    MapJoins(childObj, childType, row, depth + 1);
                }
                else
                {
                    // one-to-one
                    var existingChild = joinProperty.GetValue(parent);
                    var childObj = existingChild
                        ?? CreateMappedEntity(childType, joinData);

                    joinProperty.SetValue(parent, childObj);
                    MapJoins(childObj, childType, row, depth + 1);
                }
            }
        }

        private static void CreateJoins(Type type, object parent, int depth)
        {
            EnsureJoinDepth(depth);

            foreach (var joinProp in GetJoinProperties(type))
            {
                if (joinProp.PropertyType.IsGenericType &&
                    joinProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (joinProp.GetValue(parent) == null)
                    {
                        joinProp.SetValue(parent, CreateJoinListInstance(joinProp.PropertyType));
                    }
                }
            }
        }

        private static IEnumerable<(string TableName, PropertyInfo[] Properties)> GetTableSegments(Type type, string tableName, int depth)
        {
            EnsureJoinDepth(depth);

            yield return (tableName, GetScalarProperties(type).ToArray());

            foreach (var joinProp in GetJoinProperties(type))
            {
                var joinType = joinProp.GetCustomAttribute<DBJoin>()?.Type;
                if (joinType == null)
                {
                    continue;
                }

                var joinTableName = GetTableName(joinType);
                foreach (var segment in GetTableSegments(joinType, joinTableName, depth + 1))
                {
                    yield return segment;
                }
            }
        }

        private static IEnumerable<PropertyInfo> GetScalarProperties(Type type)
        {
            return type
                .GetProperties()
                .Where(p => !p.GetCustomAttributes(typeof(DBJoin), true).Any()
                    && !p.GetCustomAttributes(typeof(DbIgnore), true).Any());
        }

        private static IEnumerable<PropertyInfo> GetJoinProperties(Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(DBJoin), true).Any());
        }

        private static Type GetJoinedEntityType(Type propertyType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
            {
                return propertyType.GetGenericArguments().First();
            }

            return propertyType;
        }

        private static object CreateMappedEntity(Type type, Dictionary<string, object> values)
        {
            var entity = Activator.CreateInstance(type);
            MapEntityProperties(entity, type, values);
            CreateJoins(type, entity, 1);
            return entity;
        }

        private static void MapEntityProperties(object entity, Type type, Dictionary<string, object> values)
        {
            foreach (var prop in GetScalarProperties(type))
            {
                if (!values.ContainsKey(prop.Name) || values[prop.Name] == null || values[prop.Name] == DBNull.Value)
                {
                    continue;
                }

                try
                {
                    prop.SetValue(entity, Convert.ChangeType(values[prop.Name], prop.PropertyType));
                }
                catch (InvalidCastException)
                {
                    var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (nullableType != null)
                    {
                        prop.SetValue(entity, Convert.ChangeType(values[prop.Name], nullableType));
                    }
                }
            }
        }

        private static object FindExistingEntity(IList collection, Type entityType, Dictionary<string, object> values)
        {
            foreach (var existingEntity in collection)
            {
                bool isMatch = true;
                foreach (var prop in GetScalarProperties(entityType))
                {
                    if (!values.ContainsKey(prop.Name))
                    {
                        continue;
                    }

                    var newValue = values[prop.Name];
                    if (newValue == null || newValue == DBNull.Value)
                    {
                        continue;
                    }

                    var existingValue = prop.GetValue(existingEntity);
                    var convertedValue = ConvertValue(newValue, prop.PropertyType);

                    if (existingValue == null || !existingValue.Equals(convertedValue))
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return existingEntity;
                }
            }

            return null;
        }

        private static object ConvertValue(object value, Type propertyType)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            return Convert.ChangeType(value, targetType);
        }

        private static object CreateJoinListInstance(Type listType)
        {
            var itemType = listType.GetGenericArguments()[0];
            return Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
        }

        private static void EnsureJoinDepth(int depth)
        {
            if (depth > DBJoin.MaxDepth)
            {
                throw new InvalidOperationException($"Nested joins are limited to {DBJoin.MaxDepth} levels.");
            }
        }
    }
}
