using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public SqlQuery TruncateTable()
        {
            var newQuery = new SqlQuery();

            newQuery.Query.Append($"TRUNCATE TABLE {GetFullTableName()}");

            return newQuery;
        }
    }
}
