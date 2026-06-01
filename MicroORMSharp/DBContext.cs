using MicroORMSharp.Models;
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
    public class DBContext : IDisposable
    {
        public DBContext()
            : this(Database.GetCurrentConnectionSetup())
        {
        }

        public DBContext(string reference)
            : this(Database.GetConnectionSetup(reference))
        {
        }

        public DBContext(ServerConnections serverConnection)
        {
            if (serverConnection == null)
            {
                throw new ArgumentNullException(nameof(serverConnection));
            }

            _connection = Database.GetConnection(serverConnection);
            _databaseType = serverConnection.DatabaseType;
            _allowTableExtensions = serverConnection.AllowTableExtensions;
            Dapper = new DapperWrapper(_connection);
        }

        internal IDbConnection _connection { get; }
        internal DatabaseType _databaseType { get; }

        private bool _allowTableExtensions { get; }
        public DatabaseType DatabaseType => _databaseType;
        public DapperWrapper Dapper { get; }

        public DbQuery<T> Query<T>() where T : IMicroORMSharp
        {
            return new DbQuery<T>
            {
                _dbConnection = _connection,
                _databaseType = _databaseType,
                _allowTableExtensions = _allowTableExtensions
            };
        }

        public void WithConnection(Action<IDbConnection> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            bool closeConnection = OpenConnectionIfNeeded();
            try
            {
                action(_connection);
            }
            finally
            {
                CloseConnectionIfNeeded(closeConnection);
            }
        }

        public T WithConnection<T>(Func<IDbConnection, T> getData)
        {
            if (getData == null)
            {
                throw new ArgumentNullException(nameof(getData));
            }

            bool closeConnection = OpenConnectionIfNeeded();
            try
            {
                return getData(_connection);
            }
            finally
            {
                CloseConnectionIfNeeded(closeConnection);
            }
        }

        public async Task WithConnectionAsync(Func<IDbConnection, Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            bool closeConnection = OpenConnectionIfNeeded();
            try
            {
                await action(_connection);
            }
            finally
            {
                CloseConnectionIfNeeded(closeConnection);
            }
        }

        public async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> getData)
        {
            if (getData == null)
            {
                throw new ArgumentNullException(nameof(getData));
            }

            bool closeConnection = OpenConnectionIfNeeded();
            try
            {
                return await getData(_connection);
            }
            finally
            {
                CloseConnectionIfNeeded(closeConnection);
            }
        }

        public bool WithTransaction(Action<TransactionContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            bool closeConnection = OpenConnectionIfNeeded();
            using var transaction = _connection.BeginTransaction();

            try
            {
                action(new TransactionContext(_connection, transaction, _databaseType, _allowTableExtensions));
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                CloseConnectionIfNeeded(closeConnection);
            }
        }

        public async Task<bool> WithTransactionAsync(Func<TransactionContext, Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            bool closeConnection = OpenConnectionIfNeeded();
            using var transaction = _connection.BeginTransaction();

            try
            {
                await action(new TransactionContext(_connection, transaction, _databaseType, _allowTableExtensions));
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                CloseConnectionIfNeeded(closeConnection);
            }
        }

        #region Insert Methods
        public T Insert<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.Insert(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public void InsertOnly<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.InsertOnly(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task<T> InsertAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return await entity.InsertAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task InsertOnlyAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.InsertOnlyAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }
        #endregion

        #region Bulk Insert Methods
        public void Insert<T>(IEnumerable<T> entities) where T : IMicroORMSharp
        {
            entities.Insert(_databaseType, _connection);
        }

        public async Task InsertAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null) where T : IMicroORMSharp
        {
            await entities.InsertAsync(cancellationToken, _databaseType, _connection);
        }
        #endregion

        #region Update Methods
        public T Update<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.Update(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public T Update<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.Update(columns, cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public void UpdateOnly<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.UpdateOnly(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public void UpdateOnly<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.UpdateOnly(columns, cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task<T> UpdateAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return await entity.UpdateAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task<T> UpdateAsync<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return await entity.UpdateAsync(columns, cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task UpdateOnlyAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.UpdateOnlyAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task UpdateOnlyAsync<T>(T entity, Expression<Func<T, object>> columns, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.UpdateOnlyAsync(columns, cancellationToken, commandTimeout, _databaseType, _connection);
        }
        #endregion

        #region Delete Methods
        public void Delete<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.Delete(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task DeleteAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.DeleteAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }
        #endregion

        #region Tale Methods
        public bool TableExists<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entity.TableExists(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task<bool> TableExistsAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return await entity.TableExistsAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public bool TableExists<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return entities.TableExists(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public async Task<bool> TableExistsAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            return await entities.TableExistsAsync(cancellationToken, commandTimeout, _databaseType, _connection);
        }

        public void CreateTable<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.CreateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public async Task CreateTableAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.CreateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public void CreateTable<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entities.CreateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public async Task CreateTableAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entities.CreateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public void DropTable<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.DropTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public async Task DropTableAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.DropTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public void DropTable<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entities.DropTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public async Task DropTableAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entities.DropTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public void TruncateTable<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entity.TruncateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public async Task TruncateTableAsync<T>(T entity, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entity.TruncateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public void TruncateTable<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            entities.TruncateTable(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }

        public async Task TruncateTableAsync<T>(IEnumerable<T> entities, CancellationToken? cancellationToken = null, int? commandTimeout = null) where T : IMicroORMSharp
        {
            await entities.TruncateTableAsync(cancellationToken, commandTimeout, _databaseType, _allowTableExtensions, _connection);
        }
        #endregion

        private bool OpenConnectionIfNeeded()
        {
            if (_connection.State == ConnectionState.Open)
            {
                return false;
            }

            _connection.Open();
            return true;
        }

        private void CloseConnectionIfNeeded(bool closeConnection)
        {
            if (closeConnection && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
