using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace MicroORMSharp.SqlGenerator
{
    public class DbQuery<T> where T : IMicroORMSharp
    {
        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<MemberInfo> _selectClause { get; set; }

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Expression<Func<T, bool>> _whereClause { get; set; }

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? _take { get; set; }

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<Expression<Func<T, object>>, bool> _orderBy { get; set; } = new Dictionary<Expression<Func<T, object>>, bool>();

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CancellationToken? _cancellationToken { get; set; } = null;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? _commandTimeout { get; set; } = null;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDbConnection? _dbConnection { get; set; } = null;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDbTransaction? _dbTransaction { get; set; } = null;
    }

    public static class DbQueryExtensions
    {
        public static DbQuery<T> Select<T>(this DbQuery<T> dbQuery, params Expression<Func<T, object>>[] columns) where T : IMicroORMSharp
        {
            if (columns.Any())
            {
                dbQuery._selectClause = columns.Select(column => ((MemberExpression)column.Body).Member);
            }

            return dbQuery;
        }

        public static DbQuery<T> Where<T>(this DbQuery<T> dbQuery, Expression<Func<T, bool>> filter) where T : IMicroORMSharp
        {
            dbQuery._whereClause = filter ?? throw new ArgumentNullException("Where clause was null");

            return dbQuery;
        }

        public static DbQuery<T> Take<T>(this DbQuery<T> dbQuery, int count) where T : IMicroORMSharp
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("Take count must be greater than or equal to 1");
            }

            dbQuery._take = count;

            return dbQuery;
        }

        public static DbQuery<T> OrderBy<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column) where T : IMicroORMSharp
        {
            if (column == null)
            {
                throw new ArgumentNullException("Order by expression was null");
            }

            dbQuery._orderBy.Add(column, false);

            return dbQuery;
        }

        public static DbQuery<T> OrderByDescending<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column) where T : IMicroORMSharp
        {
            if (column == null)
            {
                throw new ArgumentNullException("Order by expression was null");
            }

            dbQuery._orderBy.Add(column, true);

            return dbQuery;
        }

        //Then by will act the same as OrderBy, this is just to make the code more LINQ like
        public static DbQuery<T> ThenBy<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column) where T : IMicroORMSharp
        {
            return OrderBy(dbQuery, column);
        }

        //Then by will act the same as OrderByDescending, this is just to make the code more LINQ like
        public static DbQuery<T> ThenByDescending<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column) where T : IMicroORMSharp
        {
            return OrderByDescending(dbQuery, column);
        }

        public static DbQuery<T> SetCancellationToken<T>(this DbQuery<T> dbQuery, CancellationToken cancellationToken) where T : IMicroORMSharp
        {
            dbQuery._cancellationToken = cancellationToken;
            return dbQuery;
        }

        public static DbQuery<T> SetTimeout<T>(this DbQuery<T> dbQuery, int commandTimeout) where T : IMicroORMSharp
        {
            dbQuery._commandTimeout = commandTimeout;
            return dbQuery;
        }

        public static DbQuery<T> SetConnection<T>(this DbQuery<T> dbQuery, IDbConnection dbConnection) where T : IMicroORMSharp
        {
            dbQuery._dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            return dbQuery;
        }

        public static DbQuery<T> SetTransaction<T>(this DbQuery<T> dbQuery, IDbTransaction dbTransaction) where T : IMicroORMSharp
        {
            dbQuery._dbTransaction = dbTransaction ?? throw new ArgumentNullException(nameof(dbTransaction));
            return dbQuery;
        }

        public static string GetSqlQuery<T>(this DbQuery<T> dbQuery, DatabaseType database) where T : IMicroORMSharp
        {
            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(database);
            var sqlQuery = sqlGenerator.Select(dbQuery);
            return sqlQuery.ToString();
        }

        public static Dictionary<string, object> GetSqlParameters<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            SqlGenerator<T> sqlGenerator = new SqlGenerator<T>(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.Select(dbQuery);
            return sqlQuery.Parameters;
        }
    }
}
