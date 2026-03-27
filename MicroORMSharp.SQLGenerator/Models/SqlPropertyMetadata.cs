using System;
using System.Reflection;

namespace MicroORMSharp.SqlGenerator.Models
{
    internal sealed class SqlPropertyMetadata
    {
        public PropertyInfo Property { get; set; }
        public string ColumnName { get; set; }
        public bool IsIgnored { get; set; }
        public bool IsJoin { get; set; }
        public bool IsIdentity { get; set; }
        public int? MaxLength { get; set; }
        public SqlJoinMetadata Join { get; set; }
    }

    internal sealed class SqlJoinMetadata
    {
        public Type JoinedType { get; set; }
        public string TableKey { get; set; }
        public string OtherKey { get; set; }
        public Attributes.DBJoinType JoinType { get; set; }
    }
}
