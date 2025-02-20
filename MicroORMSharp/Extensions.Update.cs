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
        public static T Update<T>(this T entity, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.UpdateRow(entity, false);

            using (IDbConnection db = Database.GetConnection())
            {
                entity = db.QueryFirst<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }

            return entity;
        }

        public static void UpdateSelect<T>(this T entity, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.UpdateRow(entity, true);
            using (IDbConnection db = Database.GetConnection())
            {
                db.Execute(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }
        }

        public static async Task<T> UpdateAsync<T>(this T entity, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.UpdateRow(entity, false);

            using (IDbConnection db = Database.GetConnection())
            {
                entity = await db.QueryFirstAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }

            return entity;
        }

        public static async void UpdateSelectAsync<T>(this T entity, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.UpdateRow(entity, true);
            using (IDbConnection db = Database.GetConnection())
            {
                await db.ExecuteAsync(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }
        }
    }
}
