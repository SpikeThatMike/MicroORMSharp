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
        #region Execute Methods
        public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.Execute(sql, param, transaction, commandTimeout, commandType));

        public int Execute(CommandDefinition command)
            => Database.WithConnection(db => db.Execute(command));

        public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnectionAsync(db => db.ExecuteAsync(sql, param, transaction, commandTimeout, commandType));

        public Task<int> ExecuteAsync(CommandDefinition command)
             => Database.WithConnectionAsync(db => db.ExecuteAsync(command));
        #endregion

        #region Query Methods
        public IEnumerable<dynamic> Query(string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.Query(sql, param, transaction, buffered, commandTimeout, commandType));

        public dynamic QueryFirst(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QueryFirst(sql, param, transaction, commandTimeout, commandType));

        public dynamic? QueryFirstOrDefault(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType));

        public dynamic QuerySingle(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QuerySingle(sql, param, transaction, commandTimeout, commandType));

        public dynamic? QuerySingleOrDefault(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType));

        public IEnumerable<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType));

        public T QueryFirst<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType));

        public T QueryFirstOrDefault<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType)) ?? default;

        public T QuerySingle<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType));

        public T QuerySingleOrDefault<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnection(db => db.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType));

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnectionAsync(db => db.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));

        public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnectionAsync(db => db.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType));

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnectionAsync(db => db.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType)) ?? default;

        public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnectionAsync(db => db.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType));

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => Database.WithConnectionAsync(db => db.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));

        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
            => Database.WithConnectionAsync(db => db.QueryAsync<T>(command));

        public Task<T> QueryFirstAsync<T>(CommandDefinition command)
            => Database.WithConnectionAsync(db => db.QueryFirstAsync<T>(command));

        public Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition command)
            => Database.WithConnectionAsync(db => db.QueryFirstOrDefaultAsync<T>(command));

        public Task<T> QuerySingleAsync<T>(CommandDefinition command)
            => Database.WithConnectionAsync(db => db.QuerySingleAsync<T>(command));

        public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command)
            => Database.WithConnectionAsync(db => db.QuerySingleOrDefaultAsync<T>(command));
        #endregion
    }
}
