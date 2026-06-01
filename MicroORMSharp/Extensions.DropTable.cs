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
        public static void DropTable<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            DropTable(entity, cancellationToken, commandTimeout, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static void DropTable<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            bool allowTableExtensions,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (!allowTableExtensions)
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.DropTable();

            WithConnection(db =>
            {
                db.Execute(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }, dbConnection, dbTransaction);
        }

        public static Task DropTableAsync<T>(
            this T entity,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return DropTableAsync(entity, cancellationToken, commandTimeout, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static Task DropTableAsync<T>(
            this T entity,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            bool allowTableExtensions,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            if (!allowTableExtensions)
            {
                throw new Exception("Table extensions are disabled. Add allowTableExtensions: true to Database.AddConnectionString");
            }

            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(databaseType);
            var sqlQuery = sqlGenerator.DropTable();

            return WithConnectionAsync(async db =>
            {
                await db.ExecuteAsync(new CommandDefinition(
                    sqlQuery.ToString(),
                    parameters: sqlQuery.Parameters,
                    cancellationToken: cancellationToken ?? Database._defaultCancellationToken,
                    commandTimeout: commandTimeout ?? Database._defaultCommandTimeout,
                    transaction: dbTransaction
                ));
            }, dbConnection, dbTransaction);
        }


        //List methods
        public static void DropTable<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            DropTable(entities, cancellationToken, commandTimeout, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static void DropTable<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            bool allowTableExtensions,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            DropTable(
                entity,
                cancellationToken,
                commandTimeout,
                databaseType,
                allowTableExtensions,
                dbConnection,
                dbTransaction
            );
        }

        public static Task DropTableAsync<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken = null,
            int? commandTimeout = null
        ) where T : IMicroORMSharp
        {
            return DropTableAsync(entities, cancellationToken, commandTimeout, Database.GetDatabaseType(), Database.GetTableExtensionsOption());
        }

        internal static Task DropTableAsync<T>(
            this IEnumerable<T> entities,
            CancellationToken? cancellationToken,
            int? commandTimeout,
            DatabaseType databaseType,
            bool allowTableExtensions,
            IDbConnection? dbConnection = null,
            IDbTransaction? dbTransaction = null
        ) where T : IMicroORMSharp
        {
            T entity = entities.FirstOrDefault();
            entity ??= (T)Activator.CreateInstance(typeof(T));

            return DropTableAsync(
                entity,
                cancellationToken,
                commandTimeout,
                databaseType,
                allowTableExtensions,
                dbConnection,
                dbTransaction
            );
        }
    }
}
