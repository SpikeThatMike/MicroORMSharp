using System;
using System.Globalization;

namespace MicroORMSharp.SqlGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DbDefault : Attribute
    {
        public string Value { get; }

        public DbDefault(string value)
        {
            Value = value;
        }

        public DbDefault(bool value)
        {
            Value = value ? bool.TrueString : bool.FalseString;
        }

        public DbDefault(byte value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public DbDefault(short value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public DbDefault(int value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public DbDefault(long value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public DbDefault(float value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public DbDefault(double value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public DbDefault(decimal value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
