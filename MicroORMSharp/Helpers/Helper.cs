using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MicroORMSharp.Helpers
{
    public static class Helper
    {
        public static string GetTableName<T>(DatabaseType databaseType = DatabaseType.None) where T : IMicroORMSharp
        {
            databaseType = databaseType != DatabaseType.None ? databaseType : Database.GetDatabaseType();
            var sqlGenerator = new SqlGenerator<T>(databaseType);

            return sqlGenerator.GetFullTableName();
        }
    }
}
