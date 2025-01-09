using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORMSharp.SqlGenerator
{
    public class SqlQuery
    {
        public StringBuilder Query { get; set; } = new StringBuilder();
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        #region Constructors
        public SqlQuery() { }

        public SqlQuery(string query)
        {
            Query.Append(query);
        }

        public SqlQuery(Dictionary<string, object> parameters)
        {
            Parameters = parameters;
        }

        public SqlQuery(string query, Dictionary<string, object> parameters)
        {
            Parameters = parameters;
            Query.Append(query);
        }
        #endregion

        public override string ToString()
        {
            return Query.ToString();
        }

        public static SqlQuery IsSql(string sql)
        {
            return new SqlQuery() { Query = new StringBuilder(sql) };
        }

        public static SqlQuery IsParameter(int count, object value)
        {
            return new SqlQuery() { Query = new StringBuilder($"@p{count}"), Parameters = new Dictionary<string, object> { { count.ToString(), value } } };
        }

        public static SqlQuery IsCollection(ref int countStart, IEnumerable values)
        {
            var parameters = new Dictionary<string, object>();
            var sql = new StringBuilder("(");
            foreach (var value in values)
            {
                parameters.Add(countStart.ToString(), value);
                sql.Append($"@p{countStart},");
                countStart++;
            }

            if (sql.Length == 1)
            {
                sql.Append("null,");
            }

            sql[sql.Length - 1] = ')';
            return new SqlQuery() { Query = new StringBuilder(sql.ToString()), Parameters = parameters };
        }

        public static SqlQuery Concat(string @operator, SqlQuery operand)
        {
            return new SqlQuery() { Query = new StringBuilder($"({@operator} {operand.Query.ToString()})"), Parameters = operand.Parameters };
        }

        public static SqlQuery Concat(SqlQuery left, string @operator, SqlQuery right)
        {
            if (right.ToString().Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
            {
                @operator = @operator == "=" ? "IS" : "IS NOT";
            }

            left.Parameters.ToList().ForEach(x => right.Parameters.Add(x.Key, x.Value));

            return new SqlQuery() { Query = new StringBuilder($"({left.ToString()} {@operator} {right.ToString()})"), Parameters = right.Parameters };
        }
    }
}
