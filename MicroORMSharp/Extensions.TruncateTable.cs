using Dapper;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Extensions
    {
        public static void TruncateTable<T>(this T entity, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
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

        public static async Task TruncateTableAsync<T>(this T entity, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
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

        public static void TruncateTable<T>(this IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            TruncateTable(entity, cancellationToken);
        }

        public static async Task TruncateTableAsync<T>(this IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            await TruncateTableAsync(entity, cancellationToken);
        }
    }
}
