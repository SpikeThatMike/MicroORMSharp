using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        public static bool Any<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(_currentConnection.DatabaseType);
            var sqlQuery = sqlGenerator.Select(dbQuery);

            //Use QueryFirstOrDefault to avoid loading all the data instead of using Any()
            T result;
            using (IDbConnection db = GetConnection())
            {
                result = db.QueryFirstOrDefault<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken
                ));
            }

            return result != null;
        }

        public static async Task<bool> AnyAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(_currentConnection.DatabaseType);
            var sqlQuery = sqlGenerator.Select(dbQuery);

            //Use QueryFirstOrDefault to avoid loading all the data instead of using Any()
            T result;
            using (IDbConnection db = GetConnection())
            {
                result = await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    commandTimeout: dbQuery._commandTimeout ?? _defaultCommandTimeout,
                    cancellationToken: dbQuery._cancellationToken
                ));
            }

            return result != null;
        }
    }
}
