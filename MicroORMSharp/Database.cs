using MicroORMSharp.ExampleModels;
using MicroORMSharp.Models;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        private static List<ServerConnections> _connections = new List<ServerConnections>();
        private static ServerConnections _currentConnection;
        private static string _defaultDatabaseSchema;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int _defaultCommandTimeout = 30;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static CancellationToken _defaultCancellationToken = default;

        private static DapperWrapper _dapperWrapper = null;
        public static DapperWrapper Dapper
        {
            get
            {
                _dapperWrapper ??= new DapperWrapper();
                return _dapperWrapper;
            }
            private set
            {
                _dapperWrapper = value;
            }
        }

        public static void Initialise()
        {
            SqlGeneratorCache.Initialise();
        }

        public static DbQuery<T> Query<T>() where T : IMicroORMSharp
        {
            return new DbQuery<T>();
        }

        public static DBContext CreateContext()
        {
            return new DBContext(GetCurrentConnectionSetup());
        }

        public static DBContext CreateContext(string reference)
        {
            return new DBContext(GetConnectionSetup(reference));
        }

        public static DBContext CreateContext(ServerConnections serverConnection)
        {
            return new DBContext(serverConnection);
        }

        #region Connections
        public static IDbConnection GetConnection()
        {
            return GetConnection(GetCurrentConnectionSetup());
        }

        public static IDbConnection GetConnection(string reference)
        {
            return GetConnection(GetConnectionSetup(reference));
        }

        public static IDbConnection GetConnection(ServerConnections connection)
        {
            if (connection == null)
            {
                throw new Exception("No connection setup found");
            }

            return connection.DatabaseType switch
            {
                DatabaseType.MySql => new MySqlConnection(connection.ConnectionString),
                DatabaseType.SqlServer => new SqlConnection(connection.ConnectionString),
                _ => throw new ArgumentException($"Unsupported database type connection: {connection.DatabaseType}")
            };
        }

        internal static ServerConnections GetCurrentConnectionSetup()
        {
            if (_currentConnection == null)
            {
                throw new Exception("No connection string set");
            }

            return _currentConnection;
        }

        internal static ServerConnections GetConnectionSetup(string reference)
        {
            if (!_connections.Any(x => x.Reference == reference))
            {
                throw new Exception("No connection string with this reference");
            }

            return _connections.First(x => x.Reference == reference);
        }

        public static T WithConnection<T>(Func<IDbConnection, T> getData)
        {
            using var db = GetConnection();
            db.Open();
            return getData(db);
        }

        public static async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> getData)
        {
            using var db = GetConnection();
            db.Open();
            return await getData(db);
        }
        #endregion

        #region Connection String Management
        public static void AddConnectionString(DatabaseType databaseType, string reference, string sqlConnection, bool allowTableExtensions = false, bool connectionTest = true)
        {
            if (_connections.Any(x => x.Reference == reference))
            {
                throw new Exception("Connection reference already exists");
            }

            IDbConnection result = databaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(sqlConnection),
                DatabaseType.MySql => new MySqlConnection(sqlConnection),
                _ => throw new Exception("Unknown value")
            };

            if (connectionTest)
            {
                try
                {
                    result.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Connection to database failed");
                }
                finally
                {
                    result.Close();
                    result.Dispose();
                }
            }

            var connection = new ServerConnections(databaseType, reference, sqlConnection, allowTableExtensions);
            _connections.Add(connection);

            if (_connections.Count == 1)
            {
                _currentConnection = connection;
            }
        }

        public static void SetConnectionString(string reference)
        {
            if (!_connections.Any(x => x.Reference == reference))
            {
                throw new Exception("Connection reference doesn't exists");
            }

            _currentConnection = _connections.FirstOrDefault(x => x.Reference == reference);
        }

        public static void RemoveConnectionString(string reference)
        {
            if (!_connections.Any(x => x.Reference == reference))
            {
                throw new Exception("Connection reference doesn't exists");
            }

            _connections.Remove(_connections.First(x => x.Reference == reference));

            if (_currentConnection.Reference == reference)
            {
                if (_connections.Count == 0)
                {
                    _currentConnection = null;
                }
                else
                {
                    _currentConnection = _connections.First();
                }
            }
        }

        public static IEnumerable<ServerConnections> GetAllConnections()
        {
            return _connections;
        }
        #endregion

        #region Defaults
        public static void SetDefaultTimeout(int timeout)
        {
            if (timeout < 0 || timeout > 86400)
            {
                throw new Exception("Query timeouts must be between 0 and 86400 seconds");
            }

            _defaultCommandTimeout = timeout;
        }

        /// <summary>
        /// Sets the default cancellation token, if a cancellation token is passed in as a parameter it will take priority over this
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <exception cref="Exception"></exception>
        public static void SetDefaultCancellationToken(CancellationToken token)
        {
            if (token == null || token.IsCancellationRequested)
            {
                throw new Exception("Invalid token provided");
            }

            _defaultCancellationToken = token;
        }
        #endregion

        #region Transactions
        public static bool WithTransaction(Action<TransactionContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using var conn = GetConnection();

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using var transaction = conn.BeginTransaction();

            try
            {
                action(new TransactionContext(conn, transaction, GetDatabaseType(), GetTableExtensionsOption()));

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }     
        }

        public static async Task<bool> WithTransactionAsync(Func<TransactionContext, Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using var conn = GetConnection();

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using var transaction = conn.BeginTransaction();

            try
            {
                await action(new TransactionContext(conn, transaction, GetDatabaseType(), GetTableExtensionsOption()));

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
        #endregion

        #region Helper methods
        public static DatabaseType GetDatabaseType()
        {
            if (_currentConnection == null)
            {
                throw new Exception("No connection set");
            }

            return _currentConnection.DatabaseType;
        }

        private static DatabaseType GetDatabaseType<T>(DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            return dbQuery._databaseType ?? GetDatabaseType();
        }

        public static bool GetTableExtensionsOption()
        {
            return _currentConnection?.AllowTableExtensions ?? false;
        }
        #endregion
    }
}
