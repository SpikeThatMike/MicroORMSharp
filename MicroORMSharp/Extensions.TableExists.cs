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
        public static bool TableExists<T>(this T entity, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.TableExists();

            bool exists = false;
            using (IDbConnection db = Database.GetConnection())
            {
                exists = db.QueryFirstOrDefault<bool>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }


            return exists;
        }

        public static async Task<bool> TableExistsAsync<T>(this T entity, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.TableExists();

            bool exists = false;
            using (IDbConnection db = Database.GetConnection())
            {
                exists = await db.QueryFirstOrDefaultAsync<bool>(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken,
                    commandTimeout: Database._defaultCommandTimeout
                ));
            }

            return exists;
        }

        public static bool TableExists<T>(this IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            return TableExists(entity, cancellationToken);
        }

        public static async Task<bool> TableExistsAsync<T>(this IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            return await TableExistsAsync(entity, cancellationToken);
        }
    }
}
