using System;
using System.Collections.Generic;
using System.Text;

namespace MicroORMSharp.SqlGenerator.Attributes
{
    public class DbColumn : Attribute
    {
        public string Name { get; set; }

        public DbColumn(string name)
        {
            Name = name;
        }
    }
}
