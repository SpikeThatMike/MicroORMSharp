using MicroORMSharp.ExampleModels;
using MicroORMSharp.Models;
using MicroORMSharp.SqlGenerator;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        public static DbQuery<T> Query<T>()
        {
            return new DbQuery<T>();
        }

        public static IDbConnection GetConnection()
        {
            if (_currentConnection == null || string.IsNullOrEmpty(_currentConnection.ConnectionString))
            {
                throw new Exception("No connection string set");
            }

            var connectionString = _currentConnection.ConnectionString;

            return _currentConnection.DatabaseType switch
            {
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                _ => throw new ArgumentException($"Unsupported database type connection: {_currentConnection.DatabaseType}")
            };
        }

        public static IDbConnection GetConnection(string reference)
        {
            if (!_connections.Any(x => x.Reference == reference))
            {
                throw new Exception("No connection string with this reference");
            }

            var connection = _connections.First(x => x.Reference == reference);

            return connection.DatabaseType switch
            {
                DatabaseType.MySql => new MySqlConnection(connection.ConnectionString),
                DatabaseType.SqlServer => new SqlConnection(connection.ConnectionString),
                _ => throw new ArgumentException($"Unsupported database type connection: {connection.DatabaseType}")
            };
        }

        public static DatabaseType GetDatabaseType()
        {
            return _currentConnection?.DatabaseType ?? DatabaseType.SqlServer;
        }

        public static void AddConnectionString(DatabaseType databaseType, string reference, string sqlConnection)
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
            var connection = new ServerConnections(databaseType, reference, sqlConnection);
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
    }
}
