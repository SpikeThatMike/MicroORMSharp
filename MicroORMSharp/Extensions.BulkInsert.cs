using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using Microsoft.Data.SqlClient;
using MySqlConnector;
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
    public static partial class Extensions
    {
        public static void Insert<T>(this IEnumerable<T> entities, IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null) where T : IMicroORMSharp
        {
            Insert(entities, dbConnection, dbTransaction, Database.GetDatabaseType());
        }

        internal static void Insert<T>(this IEnumerable<T> entities, IDbConnection? dbConnection, IDbTransaction? dbTransaction, DatabaseType databaseType) where T : IMicroORMSharp
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            if (entities.Count() == 0)
            {
                return;
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            sqlGenerator.ValidateAttributes(entities);

            WithConnection(db =>
            {
                if (db.State != ConnectionState.Open)
                {
                    db.Open();
                }

                if (db is SqlConnection)
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(db as SqlConnection, SqlBulkCopyOptions.Default, dbTransaction as SqlTransaction))
                    {
                        bulkCopy.DestinationTableName = sqlGenerator.GetFullTableName();

                        DataTable table = ConvertToDataTable(sqlGenerator, entities);

                        bulkCopy.WriteToServer(table);
                    }
                }
                else if (db is MySqlConnection)
                {
                    MySqlBulkCopy bulkCopy = new MySqlBulkCopy(db as MySqlConnection, dbTransaction as MySqlTransaction);
                    bulkCopy.DestinationTableName = sqlGenerator.GetFullTableName();

                    DataTable table = ConvertToDataTable(sqlGenerator, entities);

                    bulkCopy.WriteToServer(table);
                }
            }, dbConnection, dbTransaction);
        }

        public static async Task InsertAsync<T>(this IEnumerable<T> entities, CancellationToken? cancellationToken = null, IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null) where T : IMicroORMSharp
        {
            await InsertAsync(entities, cancellationToken, dbConnection, dbTransaction, Database.GetDatabaseType());
        }

        internal static async Task InsertAsync<T>(this IEnumerable<T> entities, CancellationToken? cancellationToken, IDbConnection? dbConnection, IDbTransaction? dbTransaction, DatabaseType databaseType) where T : IMicroORMSharp
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            if (entities.Count() == 0)
            {
                return;
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            sqlGenerator.ValidateAttributes(entities);

            await WithConnectionAsync(async db =>
            {
                if (db.State != ConnectionState.Open)
                {
                    db.Open();
                }

                if (db is SqlConnection)
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(db as SqlConnection, SqlBulkCopyOptions.Default, dbTransaction as SqlTransaction))
                    {
                        bulkCopy.DestinationTableName = sqlGenerator.GetFullTableName();

                        DataTable table = ConvertToDataTable(sqlGenerator, entities);

                        await bulkCopy.WriteToServerAsync(table, cancellationToken ?? Database._defaultCancellationToken);
                    }
                }
                else if (db is MySqlConnection)
                {
                    if (!db.ConnectionString.Contains("Allow Load Local Infile=True"))
                    {
                        throw new Exception("AllowLoadLocalInfile=True; must be included in the connection string to use bulk copy AND must be enabled on the server: SET GLOBAL local_infile=1;");
                    }
                    MySqlBulkCopy bulkCopy = new MySqlBulkCopy(db as MySqlConnection, dbTransaction as MySqlTransaction);
                    bulkCopy.DestinationTableName = sqlGenerator.GetFullTableName();

                    DataTable table = ConvertToDataTable(sqlGenerator, entities);

                    await bulkCopy.WriteToServerAsync(table, cancellationToken ?? Database._defaultCancellationToken);
                }
            }, dbConnection, dbTransaction);
        }

        private static DataTable ConvertToDataTable<T>(SqlGenerator<T> sqlGenerator, IEnumerable<T> data) where T : IMicroORMSharp
        {
            var table = new DataTable();
            foreach (var prop in sqlGenerator.Properties)
            {
                table.Columns.Add(GetPropertyName(prop), Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (var prop in sqlGenerator.Properties)
                {
                    var propName = GetPropertyName(prop);
                    row[propName] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private static string GetPropertyName(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<DbColumn>()?.Name ?? prop.Name;
        }
    }
}
