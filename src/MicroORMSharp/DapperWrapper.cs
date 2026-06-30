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
        private readonly IDbConnection? _dbConnection;
        private readonly IDbTransaction? _dbTransaction;

        internal DapperWrapper(IDbConnection? dbConnection = null, IDbTransaction? dbTransaction = null)
        {
            _dbConnection = dbConnection;
            _dbTransaction = dbTransaction;
        }

        private T UseConnection<T>(Func<IDbConnection, T> action, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var effectiveTransaction = transaction ?? _dbTransaction;
            var existingConnection = connection ?? effectiveTransaction?.Connection ?? _dbConnection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            return Database.WithConnection(action);
        }

        private Task<T> UseConnectionAsync<T>(Func<IDbConnection, Task<T>> action, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var effectiveTransaction = transaction ?? _dbTransaction;
            var existingConnection = connection ?? effectiveTransaction?.Connection ?? _dbConnection;
            if (existingConnection != null)
            {
                return action(existingConnection);
            }

            return Database.WithConnectionAsync(action);
        }

        private IDbTransaction? GetTransaction(IDbTransaction? transaction)
        {
            return transaction ?? _dbTransaction;
        }

        private CommandDefinition GetCommand(CommandDefinition command)
        {
            if (command.Transaction != null || _dbTransaction == null)
            {
                return command;
            }

            return new CommandDefinition(
                command.CommandText,
                command.Parameters,
                _dbTransaction,
                command.CommandTimeout,
                command.CommandType,
                command.Flags,
                command.CancellationToken
            );
        }

        #region Execute Methods
        public int Execute(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.Execute(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public int Execute(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnection(db => db.Execute(effectiveCommand), connection, effectiveCommand.Transaction);
        }

        public Task<int> ExecuteAsync(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnectionAsync(db => db.ExecuteAsync(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public Task<int> ExecuteAsync(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnectionAsync(db => db.ExecuteAsync(effectiveCommand), connection, effectiveCommand.Transaction);
        }
        #endregion

        #region Query Methods
        public IEnumerable<dynamic> Query(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.Query(sql, param, effectiveTransaction, buffered, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public dynamic QueryFirst(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QueryFirst(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public dynamic? QueryFirstOrDefault(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QueryFirstOrDefault(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public dynamic QuerySingle(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QuerySingle(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public dynamic? QuerySingleOrDefault(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QuerySingleOrDefault(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public IEnumerable<T> Query<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.Query<T>(sql, param, effectiveTransaction, buffered, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public T QueryFirst<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QueryFirst<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public T QueryFirstOrDefault<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QueryFirstOrDefault<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction) ?? default;
        }

        public T QuerySingle<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QuerySingle<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public T QuerySingleOrDefault<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnection(db => db.QuerySingleOrDefault<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnectionAsync(db => db.QueryAsync<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnectionAsync(db => db.QueryFirstAsync<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnectionAsync(db => db.QueryFirstOrDefaultAsync<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction) ?? default;
        }

        public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnectionAsync(db => db.QuerySingleAsync<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbConnection? connection = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var effectiveTransaction = GetTransaction(transaction);
            return UseConnectionAsync(db => db.QuerySingleOrDefaultAsync<T>(sql, param, effectiveTransaction, commandTimeout, commandType), connection, effectiveTransaction);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnectionAsync(db => db.QueryAsync<T>(effectiveCommand), connection, effectiveCommand.Transaction);
        }

        public Task<T> QueryFirstAsync<T>(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnectionAsync(db => db.QueryFirstAsync<T>(effectiveCommand), connection, effectiveCommand.Transaction);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnectionAsync(db => db.QueryFirstOrDefaultAsync<T>(effectiveCommand), connection, effectiveCommand.Transaction);
        }

        public Task<T> QuerySingleAsync<T>(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnectionAsync(db => db.QuerySingleAsync<T>(effectiveCommand), connection, effectiveCommand.Transaction);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command, IDbConnection? connection = null)
        {
            var effectiveCommand = GetCommand(command);
            return UseConnectionAsync(db => db.QuerySingleOrDefaultAsync<T>(effectiveCommand), connection, effectiveCommand.Transaction);
        }
        #endregion
    }
}
