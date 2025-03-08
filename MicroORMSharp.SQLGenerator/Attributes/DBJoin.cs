using System;
using System.Collections.Generic;
using System.Text;

namespace MicroORMSharp.SqlGenerator.Attributes
{
    public class DBJoin : Attribute
    {
        public Type Type { get; set; }
        public string TableKey { get; set; }
        public string OtherKey { get; set; }
        public DBJoin(Type type, string tableKey, string otherKey)
        {
            Type = type;
            TableKey = tableKey;
            OtherKey = otherKey;
        }
    }
}
