using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public sealed class TransactionDapperWrapper
    {
        private readonly DapperWrapper _dapper;

        internal TransactionDapperWrapper(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            _dapper = new DapperWrapper(dbConnection, dbTransaction);
        }

        private void ThrowIfTransactionProvided(CommandDefinition command)
        {
            if (command.Transaction != null)
            {
                throw new ArgumentException(
                    "TransactionDapperWrapper owns the transaction. Do not pass a transaction through CommandDefinition.",
                    nameof(command)
                );
            }
        }

        #region Execute Methods
        public int Execute(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.Execute(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public int Execute(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.Execute(command);
        }

        public Task<int> ExecuteAsync(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.ExecuteAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<int> ExecuteAsync(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.ExecuteAsync(command);
        }
        #endregion

        #region Query Methods
        public IEnumerable<dynamic> Query(string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.Query(sql, param, buffered: buffered, commandTimeout: commandTimeout, commandType: commandType);
        }

        public dynamic QueryFirst(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryFirst(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public dynamic? QueryFirstOrDefault(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryFirstOrDefault(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public dynamic QuerySingle(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QuerySingle(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public dynamic? QuerySingleOrDefault(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QuerySingleOrDefault(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public IEnumerable<T> Query<T>(string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.Query<T>(sql, param, buffered: buffered, commandTimeout: commandTimeout, commandType: commandType);
        }

        public T QueryFirst<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryFirst<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public T QueryFirstOrDefault<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public T QuerySingle<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QuerySingle<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public T QuerySingleOrDefault<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QuerySingleOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryFirstAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QuerySingleAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dapper.QuerySingleOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.QueryAsync<T>(command);
        }

        public Task<T> QueryFirstAsync<T>(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.QueryFirstAsync<T>(command);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.QueryFirstOrDefaultAsync<T>(command);
        }

        public Task<T> QuerySingleAsync<T>(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.QuerySingleAsync<T>(command);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command)
        {
            ThrowIfTransactionProvided(command);
            return _dapper.QuerySingleOrDefaultAsync<T>(command);
        }
        #endregion
    }
}
