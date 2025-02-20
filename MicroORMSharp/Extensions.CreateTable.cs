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
        public static void CreateTable<T>(this T entity, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.CreateTable();

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

        public static async Task CreateTableAsync<T>(this T entity, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            if (!Database.GetTableExtensionsOption())
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(Database.GetDatabaseType());
            var sqlQuery = sqlGenerator.CreateTable();

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

        public static void CreateTable<T>(this IEnumerable<T> entities, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            CreateTable(entity, cancellationToken);
        }

        public static async Task CreateTableAsync<T>(this IEnumerable<T> entities, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            await CreateTableAsync(entity, cancellationToken);
        }
    }
}
