using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        public static T ExecuteSingle<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType(dbQuery));
            if (sqlGenerator.JoinProperties.Any())
            {
                //Dont execute this as FirstOrDefault or if it has joins they wont worker properly
                return dbQuery.Execute().FirstOrDefault();
            }

            var sqlQuery = sqlGenerator.Select(dbQuery);
            return WithQueryConnection(db =>
            {
                return db.QueryFirstOrDefault<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    transaction: dbQuery._dbTransaction,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }, dbQuery);
        }

        public static async Task<T> ExecuteSingleAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType(dbQuery));
            if (sqlGenerator.JoinProperties.Any())
            {
                //Dont execute this as FirstOrDefault or if it has joins they wont worker properly
                return (await dbQuery.ExecuteAsync()).FirstOrDefault();
            }

            var sqlQuery = sqlGenerator.Select(dbQuery);
            return await WithQueryConnectionAsync(async db =>
            {
                return await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    transaction: dbQuery._dbTransaction,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }, dbQuery);
        }

        public static Result ExecuteSingle<T, Result>(this DbProjectionQuery<T, Result> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            var result = dbQuery.Query.ExecuteSingle();
            if (result == null)
            {
                return default!;
            }

            return dbQuery.Selector.Compile()(result);
        }

        public static async Task<Result> ExecuteSingleAsync<T, Result>(this DbProjectionQuery<T, Result> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            var result = await dbQuery.Query.ExecuteSingleAsync();
            if (result == null)
            {
                return default!;
            }

            return dbQuery.Selector.Compile()(result);
        }
    }
}
