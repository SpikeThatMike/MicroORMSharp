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
        public static T Insert<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return Insert(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static T Insert<T>(
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
            var sqlQuery = sqlGenerator.InsertRow(entity);

            return WithConnection(db => db.QueryFirst<T>(new CommandDefinition(
                sqlQuery.ToString(),
                parameters: sqlQuery.Parameters,
                cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                transaction: dbTransaction
            )), dbConnection, dbTransaction);
        }

        public static void InsertOnly<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            InsertOnly(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static void InsertOnly<T>(
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
            var sqlQuery = sqlGenerator.InsertRow(entity, false);

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

        public static Task<T> InsertAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return InsertAsync(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static Task<T> InsertAsync<T>(
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
            var sqlQuery = sqlGenerator.InsertRow(entity);

            return WithConnectionAsync(db => db.QueryFirstAsync<T>(new CommandDefinition(
                sqlQuery.ToString(),
                parameters: sqlQuery.Parameters,
                cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                transaction: dbTransaction
            )), dbConnection, dbTransaction);
        }

        public static Task InsertOnlyAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return InsertOnlyAsync(entity, cancellationToken, commandTimeout, Database.GetDatabaseType());
        }

        internal static Task InsertOnlyAsync<T>(
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
            var sqlQuery = sqlGenerator.InsertRow(entity, false);

            return WithConnectionAsync(async db =>
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
