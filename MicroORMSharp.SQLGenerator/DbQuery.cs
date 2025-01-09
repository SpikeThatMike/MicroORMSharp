using MicroORMSharp.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace MicroORMSharp.SqlGenerator
{
    public class DbQuery<T>
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
        public Dictionary<string, bool> _orderBy { get; set; } = new Dictionary<string, bool>();

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CancellationToken _cancellationToken { get; set; } = default;

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? _commandTimeout { get; set; } = null;
    }

    public static class DbQueryExtensions
    {
        public static DbQuery<T> Select<T>(this DbQuery<T> dbQuery, params Expression<Func<T, object>>[] columns)
        {
            if (columns.Any())
            {
                dbQuery._selectClause = columns.Select(column => ((MemberExpression)column.Body).Member);
            }

            return dbQuery;
        }

        public static DbQuery<T> Where<T>(this DbQuery<T> dbQuery, Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("Where clause was null");
            }

            dbQuery._whereClause = filter;

            return dbQuery;
        }

        public static DbQuery<T> Take<T>(this DbQuery<T> dbQuery, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("Take count must be greater than or equal to 1");
            }

            dbQuery._take = count;

            return dbQuery;
        }

        public static DbQuery<T> OrderBy<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("Order by expression was null");
            }

            string name = GetPropertyName(column);

            dbQuery._orderBy.Add(name, false);

            return dbQuery;
        }

        public static DbQuery<T> OrderByDescending<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("Order by expression was null");
            }

            string name = GetPropertyName(column);

            dbQuery._orderBy.Add(name, true);

            return dbQuery;
        }

        //Then by will act the same as OrderBy, this is just to make the code more LINQ like
        public static DbQuery<T> ThenBy<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column)
        {
            return OrderBy(dbQuery, column);
        }

        //Then by will act the same as OrderByDescending, this is just to make the code more LINQ like
        public static DbQuery<T> ThenByDescending<T>(this DbQuery<T> dbQuery, Expression<Func<T, object>> column)
        {
            return OrderByDescending(dbQuery, column);
        }

        public static DbQuery<T> SetCancellationToken<T>(this DbQuery<T> dbQuery, CancellationToken cancellationToken)
        {
            dbQuery._cancellationToken = cancellationToken;
            return dbQuery;
        }

        public static DbQuery<T> SetTimeout<T>(this DbQuery<T> dbQuery, int commandTimeout)
        {
            dbQuery._commandTimeout = commandTimeout;
            return dbQuery;
        }

        private static string GetPropertyName<T>(Expression<Func<T, object>> column)
        {
            var expression = (MemberExpression)column.Body;
            return expression.Member.GetCustomAttribute<DbColumn>()?.Name ?? expression.Member.Name;
        }
    }
}
