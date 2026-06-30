using System;
using System.Collections.Generic;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator.Models
{
    internal sealed class SqlMetadata
    {
        public Type EntityType { get; set; }
        public string TableDatabase { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public List<PropertyInfo> AllProperties { get; set; }
        public List<PropertyInfo> Properties { get; set; }
        public List<PropertyInfo> IgnoreProperties { get; set; }
        public List<PropertyInfo> JoinProperties { get; set; }
        public List<PropertyInfo> IdentityProperties { get; set; }
        public List<PropertyInfo> DataProperties { get; set; }
        public Dictionary<PropertyInfo, SqlPropertyMetadata> PropertyMetadata { get; set; }
    }
}
