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
            var sqlQuery = sqlGenerator.Select(dbQuery);

            IEnumerable<T> results;
            using (IDbConnection db = GetConnection())
            {
                results = db.Query<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }

            return results;
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

            IEnumerable<T> results;
            using (IDbConnection db = GetConnection())
            {
                results = await db.QueryAsync<T>(new CommandDefinition(
                   sqlQuery.ToString(),
                   parameters: sqlQuery.Parameters,
                   commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                   cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
               ));
            }

            return results;
        }

        private static async Task<IEnumerable<T>> ExecuteJoinAsync<T>(this DbQuery<T> dbQuery, SqlGenerator<T> sqlGenerator) where T : IMicroORMSharp
        {
            var sqlQuery = sqlGenerator.Select(dbQuery);

            var lookup = new Dictionary<long, T>();
            using (IDbConnection db = GetConnection())
            {
                await db.QueryAsync<dynamic>(sqlQuery.ToString(), param: sqlQuery.Parameters, commandType: CommandType.Text)
                    .ContinueWith(task =>
                    {
                        var splitOn = new List<string>();

                        var mainColumns = sqlGenerator.Properties
                            .Where(x => x.GetCustomAttribute<DbIdentity>() != null);

                        if (mainColumns.Any())
                        {
                            var column = mainColumns.FirstOrDefault();
                            splitOn.Add(column.GetCustomAttribute<DbColumn>()?.Name ?? column.Name);
                        }

                        foreach (var join in sqlGenerator.JoinProperties)
                        {
                            var dbJoin = join.GetCustomAttribute<DBJoin>();
                            var joinColumns = dbJoin.Type
                                .GetProperties()
                                .Where(x => x.GetCustomAttribute<DbIdentity>() != null);

                            if (joinColumns.Any())
                            {
                                var column = joinColumns.FirstOrDefault();
                                splitOn.Add(column.GetCustomAttribute<DbColumn>()?.Name ?? column.Name);
                            }
                        }

                        var rows = task.Result;
                        var results = new List<Dictionary<string, Dictionary<string, object>>>();
                        foreach (var row in rows)
                        {
                            var tables = new Dictionary<string, Dictionary<string, object>>();
                            var properties = (IDictionary<string, object>)row;

                            // Initialize the main table dictionary
                            var mainTableName = sqlGenerator.TableName;
                            tables[mainTableName] = new Dictionary<string, object>();

                            // Keep track of which table we're currently populating
                            string currentTableName = mainTableName;
                            int splitIndex = 0;
                            bool foundFirstId = false;

                            // Process each property in the row
                            foreach (var prop in properties)
                            {
                                // Check if this is a split column
                                if (splitOn.Contains(prop.Key))
                                {
                                    // If we've already seen a split column with this name, move to the next table
                                    if (foundFirstId && prop.Key == splitOn[splitIndex])
                                    {
                                        splitIndex++;

                                        // If we have more tables to process
                                        if (splitIndex < sqlGenerator.JoinProperties.Count() + 1)
                                        {
                                            // Get the name of the joined table
                                            currentTableName = splitIndex == 0
                                                ? mainTableName
                                                : sqlGenerator.JoinProperties.ElementAt(splitIndex - 1).GetCustomAttribute<DBJoin>().Type.Name;

                                            // Initialize dictionary for this table if needed
                                            if (!tables.ContainsKey(currentTableName))
                                            {
                                                tables[currentTableName] = new Dictionary<string, object>();
                                            }
                                        }
                                    }

                                    // Mark that we've seen the first ID column
                                    if (prop.Key == splitOn[0])
                                    {
                                        foundFirstId = true;
                                    }
                                }

                                // Add the property to the current table's dictionary
                                tables[currentTableName][prop.Key] = prop.Value;
                            }

                            // Add this row's table dictionaries to the results
                            results.Add(tables);
                        }

                        var list = MapResultsWithJoins<T>(results);

                        //List<T> mapped = new List<T>();

                        //var parentName = typeof(T).GetCustomAttribute<DbTable>()?.Name ?? typeof(T).Name;


                        //foreach (var result in results)
                        //{
                        //    var parent = Activator.CreateInstance<T>();
                        //    CreateJoins(typeof(T), parent);

                        //    var parentProperties = typeof(T).GetProperties()
                        //        .Where(p => !p.GetCustomAttributes(typeof(DBJoin), true).Any());

                        //    var parentObj = result[parentName];

                        //    foreach (var prop in parentProperties)
                        //    {
                        //        var value = parentObj[prop.Name];
                        //        if (value != null)
                        //        {
                        //            prop.SetValue(parent, Convert.ChangeType(value, prop.PropertyType));
                        //        }
                        //    }

                        //    //Map joins
                        //    foreach(var join in result.Where(x => x.Key != parentName))
                        //    {

                        //    }
                        //}

                        //var rows = task.Result;
                        //foreach (var row in rows)
                        //{
                        //    // Get parent ID (e.g., Customer ID)
                        //    long parentId = row.Id;

                        //    // Try to get the parent entity from lookup, or create new if not exists
                        //    if (!lookup.TryGetValue(parentId, out T parent))
                        //    {
                        //        // Create a new instance of the parent entity
                        //        parent = Activator.CreateInstance<T>();

                        //        // Map the parent properties
                        //        var parentProperties = typeof(T).GetProperties()
                        //            .Where(p => !p.GetCustomAttributes(typeof(DBJoin), true).Any());

                        //        foreach (var prop in parentProperties)
                        //        {
                        //            var value = ((IDictionary<string, object>)row)[prop.Name];
                        //            if (value != null)
                        //            {
                        //                prop.SetValue(parent, Convert.ChangeType(value, prop.PropertyType));
                        //            }
                        //        }

                        //        // Initialize collections for joined entities
                        //        var joinProperties = typeof(T).GetProperties()
                        //            .Where(p => p.GetCustomAttributes(typeof(DBJoin), true).Any());

                        //        foreach (var joinProp in joinProperties)
                        //        {
                        //            if (joinProp.PropertyType.IsGenericType &&
                        //                joinProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        //            {
                        //                var listType = joinProp.PropertyType.GetGenericArguments()[0];
                        //                var listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
                        //                joinProp.SetValue(parent, listInstance);
                        //            }
                        //        }

                        //        lookup.Add(parentId, parent);
                        //    }

                        //    // Now handle child entities (e.g., Orders)
                        //    var childJoinProperties = typeof(T).GetProperties()
                        //        .Where(p => p.GetCustomAttributes(typeof(DBJoin), true).Any());

                        //    foreach (var joinProp in childJoinProperties)
                        //    {
                        //        var joinAttr = (DBJoin)joinProp.GetCustomAttributes(typeof(DBJoin), true).First();
                        //        var childType = joinAttr.Type;

                        //        if (row.CustomerId != null)  // This assumes Order.CustomerId is always included in results
                        //        {
                        //            // Create child instance and map properties
                        //            var child = Activator.CreateInstance(childType);
                        //            var childProperties = childType.GetProperties();

                        //            foreach (var childProp in childProperties)
                        //            {
                        //                string columnName = childProp.Name;
                        //                if (((IDictionary<string, object>)row).ContainsKey(columnName))
                        //                {
                        //                    var value = ((IDictionary<string, object>)row)[columnName];
                        //                    if (value != null)
                        //                    {
                        //                        childProp.SetValue(child, Convert.ChangeType(value, childProp.PropertyType));
                        //                    }
                        //                }
                        //            }

                        //            // Add child to parent's collection
                        //            var childList = joinProp.GetValue(parent);
                        //            if (childList != null)
                        //            {
                        //                // Get the correct Add method from the List<T> type, not from the child type
                        //                var listType = joinProp.PropertyType;
                        //                var addMethod = listType.GetMethod("Add");
                        //                if (addMethod != null)
                        //                {
                        //                    addMethod.Invoke(childList, new[] { child });
                        //                }
                        //                else
                        //                {
                        //                    // Fallback approach if we can't find the Add method directly
                        //                    dynamic dynamicList = childList;
                        //                    dynamicList.Add((dynamic)child);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    });
            }

            return lookup.Values;
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
