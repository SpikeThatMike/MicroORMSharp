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
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
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
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
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
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            CreateTable(
                entity,
                cancellationToken,
                commandTimeout,
                dbConnection,
                dbTransaction
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
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            await CreateTableAsync(
                entity,
                cancellationToken,
                commandTimeout,
                dbConnection,
                dbTransaction
            );
        }
    }
}
