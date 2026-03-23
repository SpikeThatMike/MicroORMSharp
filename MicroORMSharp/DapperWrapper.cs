using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public class DapperWrapper
    {
        private static T UseConnection<T>(Func<IDbConnection, T> action, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var existingConnection = connection ?? transaction?.Connection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            return Database.WithConnection(action);
        }

        private static Task<T> UseConnectionAsync<T>(Func<IDbConnection, Task<T>> action, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var existingConnection = connection ?? transaction?.Connection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            return Database.WithConnectionAsync(action);
        }

        #region Execute Methods
        public int Execute(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.Execute(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public int Execute(CommandDefinition command, IDbConnection? connection = null)
            => UseConnection(db => db.Execute(command), connection, command.Transaction);

        public Task<int> ExecuteAsync(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnectionAsync(db => db.ExecuteAsync(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public Task<int> ExecuteAsync(CommandDefinition command, IDbConnection? connection = null)
             => UseConnectionAsync(db => db.ExecuteAsync(command), connection, command.Transaction);
        #endregion

        #region Query Methods
        public IEnumerable<dynamic> Query(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.Query(sql, param, transaction, buffered, commandTimeout, commandType), connection, transaction);

        public dynamic QueryFirst(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QueryFirst(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public dynamic? QueryFirstOrDefault(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public dynamic QuerySingle(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QuerySingle(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public dynamic? QuerySingleOrDefault(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public IEnumerable<T> Query<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType), connection, transaction);

        public T QueryFirst<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public T QueryFirstOrDefault<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction) ?? default;

        public T QuerySingle<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public T QuerySingleOrDefault<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnection(db => db.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnectionAsync(db => db.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnectionAsync(db => db.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnectionAsync(db => db.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction) ?? default;

        public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnectionAsync(db => db.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => UseConnectionAsync(db => db.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), connection, transaction);

        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command, IDbConnection? connection = null)
            => UseConnectionAsync(db => db.QueryAsync<T>(command), connection, command.Transaction);

        public Task<T> QueryFirstAsync<T>(CommandDefinition command, IDbConnection? connection = null)
            => UseConnectionAsync(db => db.QueryFirstAsync<T>(command), connection, command.Transaction);

        public Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command, IDbConnection? connection = null)
            => UseConnectionAsync(db => db.QueryFirstOrDefaultAsync<T>(command), connection, command.Transaction);

        public Task<T> QuerySingleAsync<T>(CommandDefinition command, IDbConnection? connection = null)
            => UseConnectionAsync(db => db.QuerySingleAsync<T>(command), connection, command.Transaction);

        public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command, IDbConnection? connection = null)
            => UseConnectionAsync(db => db.QuerySingleOrDefaultAsync<T>(command), connection, command.Transaction);
        #endregion
    }
}
