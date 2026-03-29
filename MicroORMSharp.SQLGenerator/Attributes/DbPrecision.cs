using System;

namespace MicroORMSharp.SqlGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DbPrecision : Attribute
    {
        public int Precision { get; }
        public int Scale { get; }

        public DbPrecision(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
