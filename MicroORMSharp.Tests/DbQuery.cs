using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public sealed class DbQuery
    {
        [TestMethod]
        public void DbQuery_Take_SetsTakeValue()
        {
            var query = new DbQuery<Customers>().Take(1);

            Assert.AreEqual(1, query._take, "Incorrect take limit inside of select query");
        }

        [TestMethod]
        public void DbQuery_Select_SetsSelectedColumns()
        {
            var query = new DbQuery<Customers>()
                .Select(x => x.AddressLine1, x => x.AddressLine2, x => x.AddressLine3, x => x.AddressLine4, x => x.Postcode);

            var columns = query._selectClause.Select(x => x.GetCustomAttribute<DbColumn>()?.Name ?? x.Name);
            var expected = new List<string> { "AddressLine1", "AddressLine2", "AddressLine3", "AddressLine4", "Postalcode" };

            CollectionAssert.AreEqual(expected, columns.ToList(), "Incorrect columns inside of select query");
        }

        [TestMethod]
        public void DbQuery_Where_SetsWhereClause()
        {
            var query = new DbQuery<Customers>()
                .Where(x => x.Active && x.AddressLine1 == "Test Street");

            Expression<Func<Customers, bool>> expected = x => x.Active && x.AddressLine1 == "Test Street";

            Assert.AreEqual(expected.ToString(), query._whereClause.ToString(), "Incorrect where clause inside of select query");
        }

        [TestMethod]
        public void DbQuery_OrderBy_SetsColumnsInOrder()
        {
            var query = new DbQuery<Customers>()
                .OrderBy(x => x.Forename)
                .ThenByDescending(x => x.Surname);

            var expected = new List<KeyValuePair<string, bool>>
            {
                new("Forename", false),
                new("Surname", true)
            };

            var actual = query._orderBy
                .Select(x =>
                {
                    var expression = (MemberExpression)x.Key.Body;
                    var name =  ((PropertyInfo)expression.Member).GetCustomAttribute<DbColumn>()?.Name ?? ((PropertyInfo)expression.Member).Name;
                    return new KeyValuePair<string, bool>(name, x.Value);
                })
                .ToList();

            CollectionAssert.AreEqual(expected, actual, "Incorrect order by inside select query");
        }
    }
}
