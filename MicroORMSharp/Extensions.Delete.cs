using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Extensions
    {
        public static void Delete<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            Delete(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static void Delete<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.DeleteRow(entity);

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

        public static async Task DeleteAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            await DeleteAsync(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static async Task DeleteAsync<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.DeleteRow(entity);

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
    }
}
