using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MicroORMSharp.SqlGenerator.Models
{
    internal class SqlMetadata
    {
        public string TableDatabase { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }

        public List<PropertyInfo> AllProperties { get; set; }
        public List<PropertyInfo> Properties { get; set; }
        public List<PropertyInfo> IgnoreProperties { get; set; }
        public List<PropertyInfo> JoinProperties { get; set; }

        public string FullTableNameSqlServer { get; set; }
        public string FullTableNameMySql { get; set; }
    }
}
