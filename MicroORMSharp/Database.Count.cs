using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        //This needs to be refactored to use COUNT(*) instead
        public static int Count<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            var sqlQuery = sqlGenerator.Select(dbQuery);

            IEnumerable<T> results;
            using (IDbConnection db = GetConnection())
            {
                results = db.Query<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }

            return results.Count();
        }

        //This needs to be refactored to use COUNT(*) instead
        public static async Task<int> CountAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(GetDatabaseType());
            var sqlQuery = sqlGenerator.Select(dbQuery);

            IEnumerable<T> results;
            using (IDbConnection db = GetConnection())
            {
                results = await db.QueryAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken ?? _defaultCancellationToken
                ));
            }

            return results.Count();
        }
    }
}
