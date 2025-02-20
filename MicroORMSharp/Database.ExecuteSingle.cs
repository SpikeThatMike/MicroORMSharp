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
    public static partial class Database
    {
        public static T ExecuteSingle<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            var sqlQuery = sqlGenerator.Select(dbQuery);

            T result;
            using (IDbConnection db = GetConnection())
            {
                result = db.QueryFirstOrDefault<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }

            return result;
        }

        public static async Task<T> ExecuteSingleAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            var sqlQuery = sqlGenerator.Select(dbQuery);

            T result;
            using (IDbConnection db = GetConnection())
            {
                result = await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                   sqlQuery.ToString(),
                   parameters: sqlQuery.Parameters,
                   commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                   cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
               ));
            }

            return result;
        }
    }
}
