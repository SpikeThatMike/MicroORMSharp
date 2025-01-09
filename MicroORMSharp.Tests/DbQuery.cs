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
        public void DbQuery_Take()
        {
            var query = new DbQuery<Customers>()
                .Take(1);

            Assert.IsTrue(
                query._take == 1,
                "Incorrect take limit inside of select query"
            );
        }

        [TestMethod]
        public void DbQuery_Select()
        {
            var query = new DbQuery<Customers>()
                .Select(x => x.AddressLine1, x => x.AddressLine2, x => x.AddressLine3, x => x.AddressLine4, x => x.Postcode);

            var columns = query._selectClause.Select(x => x.GetCustomAttribute<DbColumn>()?.Name ?? x.Name);

            Assert.IsTrue(
                new List<string> { "AddressLine1", "AddressLine2", "AddressLine3", "AddressLine4", "Postalcode" }.SequenceEqual(columns),
                "Incorrect columns inside of select query"
            );
        }

        [TestMethod]
        public void DbQuery_Where()
        {
            var query = new DbQuery<Customers>()
                .Where(x => x.Active && x.AddressLine1 == "Test Street");

            var where = query._whereClause;
            Expression<Func<Customers, bool>> filter = x => x.Active && x.AddressLine1 == "Test Street";

            Assert.IsTrue(
                filter.ToString() == where.ToString(),
                "Incorrect where clause inside of select query"
            );
        }

        [TestMethod]
        public void DbQuery_OrderBy()
        {
            var query = new DbQuery<Customers>()
                .OrderBy(x => x.Forename).ThenByDescending(x => x.Surname);

            var columns = query._orderBy;
            var actualColumns = new Dictionary<string, bool>
            {
                { "Forename", false },
                { "Surname", true }
            };

            Assert.IsTrue(
                columns.SequenceEqual(actualColumns),
                "Incorrect order by inside select query"
            );
        }
    }
}
