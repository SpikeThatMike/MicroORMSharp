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
            int? commandTimeout = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.InsertRow(entity);

            using (IDbConnection db = dbTransaction?.Connection ?? Database.GetConnection())
            {
                entity = db.QueryFirst<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }

            return entity;
        }

        public static void InsertOnly<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.InsertRow(entity, false);
            using (IDbConnection db = dbTransaction?.Connection ?? Database.GetConnection())
            {
                db.Execute(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }
        }

        public static async Task<T> InsertAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.InsertRow(entity);

            using (IDbConnection db = dbTransaction?.Connection ?? Database.GetConnection())
            {
                entity = await db.QueryFirstAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }

            return entity;
        }

        public static async void InsertOnlyAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.InsertRow(entity, false);
            using (IDbConnection db = dbTransaction?.Connection ?? Database.GetConnection())
            {
                await db.ExecuteAsync(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }
        }
    }
}
