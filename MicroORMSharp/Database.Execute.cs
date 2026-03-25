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

        private static TResult WithQueryConnection<T, TResult>(Func<IDbConnection, TResult> action, DbQuery<T> dbQuery)
        {
            var existingConnection = dbQuery._dbConnection ?? dbQuery._dbTransaction?.Connection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            return WithConnection(action);
        }

        private static async Task<TResult> WithQueryConnectionAsync<T, TResult>(Func<IDbConnection, Task<TResult>> action, DbQuery<T> dbQuery)
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
            var tableSegments = new List<(string TableName, PropertyInfo[] Properties)>
            {
                (sqlGenerator.TableName, sqlGenerator.Properties.ToArray())
            };

            foreach (var join in sqlGenerator.JoinProperties)
            {
                var dbJoin = join.GetCustomAttribute<DBJoin>();
                tableSegments.Add((GetTableName(dbJoin.Type), dbJoin.Type.GetProperties()));
            }

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

            foreach (var result in results)
            {
                var parentObj = result[parentName];

                var parentProperties = typeof(T).GetProperties()
                    .Where(p => !p.GetCustomAttributes(typeof(DBJoin), true).Any());

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
                    CreateJoins(typeof(T), parent);

                    // Add to our list of tracked parents
                    mappedParents.Add(parent);
                }

                // Process joins
                MapJoins(parent, result, parentName);
            }

            return mappedParents;
        }

        private static void MapJoins<T>(T parent, Dictionary<string, Dictionary<string, object>> row, string parentName)
        {
            foreach (var join in row.Where(x => x.Key != parentName))
            {
                string joinName = join.Key;
                var joinData = join.Value;

                // data is empty or all null
                if (joinData == null || joinData.All(kvp => kvp.Value == null || kvp.Value == DBNull.Value))
                    continue;

                var joinProperty = typeof(T).GetProperties()
                    .FirstOrDefault(p =>
                        (p.GetCustomAttribute<DBJoin>()?.Type.GetCustomAttribute<DbTable>().Name ?? p.GetCustomAttribute<DBJoin>()?.Type.Name) == joinName ||
                        p.Name == joinName);

                if (joinProperty == null)
                    continue; // Skip if no matching property

                var joinAttr = joinProperty.GetCustomAttribute<DBJoin>();
                if (joinAttr == null)
                    continue;

                if (typeof(IEnumerable).IsAssignableFrom(joinProperty.PropertyType) && joinProperty.PropertyType != typeof(string))
                {
                    // one-to-many
                    Type childType = joinProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                    if (childType == null)
                        continue;

                    // child object
                    var childObj = Activator.CreateInstance(childType);

                    // Map the child properties
                    foreach (var prop in childType.GetProperties())
                    {
                        if (joinData.ContainsKey(prop.Name) && joinData[prop.Name] != null && joinData[prop.Name] != DBNull.Value)
                        {
                            try
                            {
                                prop.SetValue(childObj, Convert.ChangeType(joinData[prop.Name], prop.PropertyType));
                            }
                            catch (InvalidCastException)
                            {
                                // Handle nullable types
                                if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                {
                                    prop.SetValue(childObj, Convert.ChangeType(joinData[prop.Name],
                                        Nullable.GetUnderlyingType(prop.PropertyType)));
                                }
                            }
                        }
                    }

                    // Add to the collection
                    var collection = joinProperty.GetValue(parent) as System.Collections.IList;
                    if (collection != null)
                    {
                        // Check if this child is already in the collection (prevent duplicates)
                        bool isDuplicate = false;
                        var childProperties = childType.GetProperties();

                        foreach (var existingChild in collection)
                        {
                            bool isMatch = true;
                            foreach (var prop in childProperties)
                            {
                                var newValue = prop.GetValue(childObj);
                                var existingValue = prop.GetValue(existingChild);

                                // Skip null values in the comparison
                                if (newValue == null)
                                    continue;

                                // Compare the values
                                if (existingValue == null || !newValue.Equals(existingValue))
                                {
                                    isMatch = false;
                                    break;
                                }
                            }

                            if (isMatch)
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate)
                            collection.Add(childObj);
                    }
                }
                else
                {
                    // one-to-one
                    Type childType = joinProperty.PropertyType;

                    // Skip if child data is null (no joined record)
                    if (joinData.All(kvp => kvp.Value == null || kvp.Value == DBNull.Value))
                        continue;

                    var childObj = Activator.CreateInstance(childType);

                    // Map the child properties
                    foreach (var prop in childType.GetProperties())
                    {
                        if (joinData.ContainsKey(prop.Name) && joinData[prop.Name] != null && joinData[prop.Name] != DBNull.Value)
                        {
                            try
                            {
                                prop.SetValue(childObj, Convert.ChangeType(joinData[prop.Name], prop.PropertyType));
                            }
                            catch (InvalidCastException)
                            {
                                // Handle nullable types
                                if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                {
                                    prop.SetValue(childObj, Convert.ChangeType(joinData[prop.Name],
                                        Nullable.GetUnderlyingType(prop.PropertyType)));
                                }
                            }
                        }
                    }

                    // Set the property
                    joinProperty.SetValue(parent, childObj);
                }
            }
        }

        private static void CreateJoins(Type type, object parent)
        {
            var joinProperties = type.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(DBJoin), true).Any());

            foreach (var joinProp in joinProperties)
            {
                var nestedJoin = joinProp.GetType().GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof(DBJoin), true).Any());

                if (nestedJoin.Any())
                {
                    CreateJoins(nestedJoin.First().GetType(), nestedJoin.First());
                    continue;
                }

                if (joinProp.PropertyType.IsGenericType &&
                    joinProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = joinProp.PropertyType.GetGenericArguments()[0];
                    var listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
                    joinProp.SetValue(parent, listInstance);
                }
            }
        }
    }
}
