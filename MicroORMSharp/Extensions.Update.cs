using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Extensions
    {
        #region Synchronous methods
        public static T Update<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            return ExecuteUpdate(entity, columns: null, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }

        public static T Update<T>(
            this T entity,
            Expression<Func<T, object>> columns,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            return ExecuteUpdate(entity, columns, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }

        public static void UpdateOnly<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            ExecuteUpdateOnly(entity, columns: null, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }

        public static void UpdateOnly<T>(
            this T entity,
            Expression<Func<T, object>> columns,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            ExecuteUpdateOnly(entity, columns, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }
        #endregion

        #region Asynchronous methods
        public static async Task<T> UpdateAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            return await ExecuteUpdateAsync(entity, columns: null, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }

        public static async Task<T> UpdateAsync<T>(
            this T entity,
            Expression<Func<T, object>> columns,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            return await ExecuteUpdateAsync(entity, columns, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }

        public static async Task UpdateOnlyAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            await ExecuteUpdateOnlyAsync(entity, columns: null, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }

        public static async Task UpdateOnlyAsync<T>(
            this T entity,
            Expression<Func<T, object>> columns,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            await ExecuteUpdateOnlyAsync(entity, columns, cancellationToken, commandTimeout, dbConnection, dbTransaction);
        }
        #endregion

        #region Private execution methods
        private static T ExecuteUpdate<T>(
            T entity,
            Expression<Func<T, object>>? columns = null,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = columns == null
                ? sqlGenerator.UpdateRow(entity, true)
                : sqlGenerator.UpdateRow(entity, columns, true);

            return WithConnection(db =>
            {
                return db.QueryFirst<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }, dbConnection, dbTransaction);
        }

        private static async Task<T> ExecuteUpdateAsync<T>(
            T entity,
            Expression<Func<T, object>>? columns = null,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = columns == null
                ? sqlGenerator.UpdateRow(entity, true)
                : sqlGenerator.UpdateRow(entity, columns, true);

            return await WithConnectionAsync(async db =>
            {
                return await db.QueryFirstAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }, dbConnection, dbTransaction);
        }

        private static void ExecuteUpdateOnly<T>(
            T entity,
            Expression<Func<T, object>>? columns = null,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = columns == null
                ? sqlGenerator.UpdateRow(entity, false)
                : sqlGenerator.UpdateRow(entity, columns, false);

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

        private static async Task ExecuteUpdateOnlyAsync<T>(
            T entity,
            Expression<Func<T, object>>? columns = null,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = columns == null
                ? sqlGenerator.UpdateRow(entity, false)
                : sqlGenerator.UpdateRow(entity, columns, false);

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
        #endregion
    }
}
