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
        public static void TruncateTable<T>(CancellationToken cancellationToken) where T : IMicroORMSharp
        {
            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database._databaseType);
            var sqlQuery = sqlGenerator.TruncateTable();

            using (IDbConnection db = Database.GetConnection())
            {
                db.Execute(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }
        }

        public static async Task TruncateTableAsync<T>(CancellationToken cancellationToken) where T : IMicroORMSharp
        {
            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database._databaseType);
            var sqlQuery = sqlGenerator.TruncateTable();

            using (IDbConnection db = Database.GetConnection())
            {
                await db.ExecuteAsync(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }
        }
    }
}
