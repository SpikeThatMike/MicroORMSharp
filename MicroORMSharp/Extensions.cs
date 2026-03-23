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
    public static partial class Extensions
    {
        private static void WithConnection(Action<IDbConnection> action, IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null)
        {
            var existingConnection = dbConnection ?? dbTransaction?.Connection;
            if (existingConnection != null)
            {
                action(existingConnection);
                return;
            }

            using IDbConnection db = Database.GetConnection();
            action(db);
        }

        private static async Task WithConnectionAsync(Func<IDbConnection, Task> action, IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null)
        {
            var existingConnection = dbConnection ?? dbTransaction?.Connection;
            if (existingConnection != null)
            {
                await action(existingConnection);
                return;
            }

            using IDbConnection db = Database.GetConnection();
            await action(db);
        }

        private static T WithConnection<T>(Func<IDbConnection, T> action, IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null)
        {
            var existingConnection = dbConnection ?? dbTransaction?.Connection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            using IDbConnection db = Database.GetConnection();
            return action(db);
        }

        private static async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> action, IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null)
        {
            var existingConnection = dbConnection ?? dbTransaction?.Connection;
            if (existingConnection != null)
            {
                return await action(existingConnection);
            }

            using IDbConnection db = Database.GetConnection();
            return await action(db);
        }
    }
}
