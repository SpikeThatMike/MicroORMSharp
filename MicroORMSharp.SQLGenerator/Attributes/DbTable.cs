using System;
using System.Collections.Generic;
using System.Text;

namespace MicroORMSharp.SqlGenerator.Attributes
{
    public class DbTable : Attribute
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Database { get; set; }

        public DbTable(string name)
        {
            Name = name;
        }

        public DbTable(string schema, string name)
        {
            Schema = schema;
            Name = name;
        }

        public DbTable(string database, string schema, string name)
        {
            Database = database;
            Schema = schema;
            Name = name;
        }
    }
}
