using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using System.Text;

namespace MicroORMSharp.SqlGenerator
{
    public partial class SqlGenerator<T> where T : IMicroORMSharp
    {
        public SqlQuery TableExists()
        {
            var newQuery = new SqlQuery();

            var whereClause = BuildWhereClause();
            newQuery.Query.Append($"SELECT 1 FROM information_schema.TABLES {whereClause.Query.ToString()}");
            newQuery.Parameters = whereClause.Parameters;

            return newQuery;
        }

        private SqlQuery BuildWhereClause()
        {
            SqlQuery sqlQuery = new SqlQuery("WHERE");
            if (DatabaseType == DatabaseType.MySql && !string.IsNullOrEmpty(TableDatabase))
            {
                sqlQuery.Query.Append(" TABLE_SCHEMA = @databaseName AND");
                sqlQuery.Parameters.Add("databaseName", TableDatabase);
            }
            else if (DatabaseType == DatabaseType.SqlServer)
            {
                sqlQuery.Query.Append(" TABLE_SCHEMA = @databaseSchema AND");
                sqlQuery.Parameters.Add("databaseSchema", TableSchema);
            }

            sqlQuery.Query.Append(" TABLE_NAME = @databaseTable");
            sqlQuery.Parameters.Add("databaseTable", TableName);

            return sqlQuery;
        }
    }
}
