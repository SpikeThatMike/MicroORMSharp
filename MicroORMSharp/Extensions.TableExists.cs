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
        public static bool TableExists<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return TableExists(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static bool TableExists<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.TableExists();

            return WithConnection(db => db.QueryFirstOrDefault<bool>(new CommandDefinition(
                sqlQuery.ToString(),
                parameters: sqlQuery.Parameters,
                cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                transaction: dbTransaction
            )), dbConnection, dbTransaction);
        }

        public static async Task<bool> TableExistsAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return await TableExistsAsync(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static async Task<bool> TableExistsAsync<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.TableExists();

            return await WithConnectionAsync(db => db.QueryFirstOrDefaultAsync<bool>(new CommandDefinition(
                sqlQuery.ToString(),
                parameters: sqlQuery.Parameters,
                cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                transaction: dbTransaction
            )), dbConnection, dbTransaction);
        }

        //List methods
        public static bool TableExists<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return TableExists(entities, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static bool TableExists<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            return TableExists(
                entity,
                cancellationToken,
                commandTimeout,
                databaseType,
                dbConnection,
                dbTransaction
            );
        }

        public static async Task<bool> TableExistsAsync<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return await TableExistsAsync(entities, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static async Task<bool> TableExistsAsync<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            return await TableExistsAsync(
                entity,
                cancellationToken,
                commandTimeout,
                databaseType,
                dbConnection,
                dbTransaction
            );
        }
    }
}
