using MicroORMSharp.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroORMSharp.Models
{
    public class ServerConnections
    {
        public ServerConnections(DatabaseType databaseType, string reference, string sqlConnection)
        {
            this.DatabaseType = databaseType;
            this.Reference = reference;
            this.ConnectionString = sqlConnection;
        }
        public DatabaseType DatabaseType { get; set; }
        public string Reference { get; set; }
        public string ConnectionString { get; set; }
    }
}
