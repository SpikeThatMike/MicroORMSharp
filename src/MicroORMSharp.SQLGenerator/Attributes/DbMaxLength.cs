using System;
using System.Collections.Generic;
using System.Text;

namespace MicroORMSharp.SqlGenerator.Attributes
{
    public class DbMaxLength : Attribute
    {
        public int Max { get; set; }
        public DbMaxLength(int max)
        {
            Max = max;
        }
    }
}
