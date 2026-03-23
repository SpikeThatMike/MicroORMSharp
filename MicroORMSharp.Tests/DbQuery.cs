using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using System.Collections.Generic;
using System.Linq;
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

            var expected = new Dictionary<string, bool>
            {
                { "Forename", false },
                { "Surname", true }
            };

            CollectionAssert.AreEqual(expected.ToList(), query._orderBy.ToList(), "Incorrect order by inside select query");
        }
    }
}
