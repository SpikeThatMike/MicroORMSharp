using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public sealed class TransactionContext
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly DatabaseType _databaseType;
        private readonly bool _allowTableExtensions;

        internal TransactionContext(
            IDbConnection connection,
            IDbTransaction transaction,
            DatabaseType databaseType,
            bool allowTableExtensions
        )
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            _databaseType = databaseType;
            _allowTableExtensions = allowTableExtensions;
            Dapper = new TransactionDapperWrapper(_connection, _transaction);
        }

        public TransactionDapperWrapper Dapper { get; }

        public DbQuery<T> Query<T>() where T : IMicroORMSharp
        {
            return new DbQuery<T>
            {
                _dbConnection = _connection,
                _dbTransaction = _transaction,
                _databaseType = _databaseType,
                _allowTableExtensions = _allowTableExtensions
            };
        }

        #region Insert Methods
        public T Insert<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.Insert(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public void InsertOnly<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.InsertOnly(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task<T> InsertAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.InsertAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task InsertOnlyAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.InsertOnlyAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }
        #endregion

        #region Bulk Insert Methods
        public void Insert<T>(IEnumerable<T> entities) where T : IMicroORMSharp
        {
            entities.Insert(_databaseType, _connection, _transaction);
        }

        public Task InsertAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            return entities.InsertAsync(cancellationToken, _databaseType, _connection, _transaction);
        }
        #endregion

        #region Update Methods
        public T Update<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.Update(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public T Update<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.Update(columns, cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public void UpdateOnly<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.UpdateOnly(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public void UpdateOnly<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.UpdateOnly(columns, cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task<T> UpdateAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.UpdateAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task<T> UpdateAsync<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.UpdateAsync(columns, cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task UpdateOnlyAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.UpdateOnlyAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task UpdateOnlyAsync<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.UpdateOnlyAsync(columns, cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }
        #endregion

        #region Delete Methods
        public void Delete<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.Delete(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task DeleteAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.DeleteAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }
        #endregion

        #region Table Methods
        public bool TableExists<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.TableExists(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task<bool> TableExistsAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.TableExistsAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public bool TableExists<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entities.TableExists(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public Task<bool> TableExistsAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entities.TableExistsAsync(cancellationToken, commandTimeout, _databaseType, _connection, _transaction);
        }

        public void CreateTable<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.CreateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public Task CreateTableAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.CreateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public void CreateTable<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entities.CreateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public Task CreateTableAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entities.CreateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public void DropTable<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.DropTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public Task DropTableAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.DropTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public void DropTable<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entities.DropTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public Task DropTableAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entities.DropTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public void TruncateTable<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.TruncateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public Task TruncateTableAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.TruncateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public void TruncateTable<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entities.TruncateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }

        public Task TruncateTableAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entities.TruncateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection, _transaction);
        }
        #endregion
    }
}
