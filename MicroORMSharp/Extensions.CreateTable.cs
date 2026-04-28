using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Extensions
    {
        public static void CreateTable<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            CreateTable(entity, cancellationToken, commandTimeout, dbConnection, dbTransaction, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static void CreateTable<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            IDbConnection? dbConnection,
            IDbTransaction? dbTransaction,
            DatabaseType databaseType,
            bool allowTableExtensions
        ) where T : IMicroORMSharp
        {
            if (!allowTableExtensions)
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.CreateTable();

            WithConnection(db =>
            {
                db.Execute(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }, dbConnection, dbTransaction);
        }

        public static async Task CreateTableAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            await CreateTableAsync(entity, cancellationToken, commandTimeout, dbConnection, dbTransaction, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static async Task CreateTableAsync<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            IDbConnection? dbConnection,
            IDbTransaction? dbTransaction,
            DatabaseType databaseType,
            bool allowTableExtensions
        ) where T : IMicroORMSharp
        {
            if (!allowTableExtensions)
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.CreateTable();

            await WithConnectionAsync(async db =>
            {
                await db.ExecuteAsync(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }, dbConnection, dbTransaction);
        }

        //List methods
        public static void CreateTable<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            CreateTable(entities, cancellationToken, commandTimeout, dbConnection, dbTransaction, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static void CreateTable<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            IDbConnection? dbConnection,
            IDbTransaction? dbTransaction,
            DatabaseType databaseType,
            bool allowTableExtensions
        ) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            CreateTable(
                entity,
                cancellationToken,
                commandTimeout,
                dbConnection,
                dbTransaction,
                databaseType,
                allowTableExtensions
            );
        }

        public static async Task CreateTableAsync<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            await CreateTableAsync(entities, cancellationToken, commandTimeout, dbConnection, dbTransaction, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static async Task CreateTableAsync<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            IDbConnection? dbConnection,
            IDbTransaction? dbTransaction,
            DatabaseType databaseType,
            bool allowTableExtensions
        ) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            await CreateTableAsync(
                entity,
                cancellationToken,
                commandTimeout,
                dbConnection,
                dbTransaction,
                databaseType,
                allowTableExtensions
            );
        }
    }
}
