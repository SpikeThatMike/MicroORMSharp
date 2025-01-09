using MicroORMSharp.ExampleModels;
using MicroORMSharp.SqlGenerator;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
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
        private static Dictionary<string, string> _connections = new Dictionary<string, string>();
        private static string _currentConnection;
        private static string _defaultDatabaseSchema;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DatabaseType _databaseType = DatabaseType.SqlServer;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int _defaultCommandTimeout = 30;

        public static DbQuery<T> Query<T>()
        {
            return new DbQuery<T>();
        }

        public static void SetDatabaseType(DatabaseType type)
        {
            _databaseType = type;
        }

        public static void SetDatabaseType(DatabaseType type, string defaultSchema)
        {
            _databaseType = type;
            _defaultDatabaseSchema = defaultSchema;
        }

        public static IDbConnection GetConnection()
        {
            if (_currentConnection == null)
            {
                throw new Exception("No connection string set");
            }

            var connectionString = _connections[_currentConnection];

            return _databaseType switch
            {
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                _ => throw new ArgumentException($"Unsupported database type connection: {_databaseType}")
            };
        }

        public static IDbConnection GetConnection(string reference)
        {
            if (!_connections.ContainsKey(reference))
            {
                throw new Exception("No connection string with this reference");
            }

            var connectionString = _connections[reference];

            return _databaseType switch
            {
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                _ => throw new ArgumentException($"Unsupported database type connection: {_databaseType}")
            };
        }

        public static void AddConnectionString(string reference, string sqlConnection)
        {
            if (_connections.ContainsKey(reference))
            {
                throw new Exception("Connection reference already exists");
            }

            _connections.Add(reference, sqlConnection);

            if (_connections.Count == 1)
            {
                _currentConnection = reference;
            }
        }

        public static void RemoveConnectionString(string reference)
        {
            if (!_connections.ContainsKey(reference))
            {
                throw new Exception("Connection reference doesn't exists");
            }

            _connections.Remove(reference);

            if (_currentConnection == reference)
            {
                if (_connections.Count == 0)
                {
                    _currentConnection = null;
                }
                else
                {
                    _currentConnection = _connections.Keys.First();
                }
            }
        }
    }

    public class T
    {
        public void Test()
        {
            Database.Query<Customers>().Execute();
        }
    }
}
