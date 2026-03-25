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

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            if (sqlGenerator.JoinProperties.Any())
            {
                //Dont execute this as FirstOrDefault or if it has joins they wont worker properly
                return dbQuery.Execute().FirstOrDefault();
            }

            var sqlQuery = sqlGenerator.Select(dbQuery);

            using (IDbConnection db = GetConnection())
            {
                return db.QueryFirstOrDefault<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }
        }

        public static async Task<T> ExecuteSingleAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            if (sqlGenerator.JoinProperties.Any())
            {
                //Dont execute this as FirstOrDefault or if it has joins they wont worker properly
                return (await dbQuery.ExecuteAsync()).FirstOrDefault();
            }

            var sqlQuery = sqlGenerator.Select(dbQuery);

            using (IDbConnection db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }
        }
    }
}
