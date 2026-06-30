using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.Tests.Helpers;
using MicroORMSharp.Tests.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public sealed class DbQueryTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
            TestDatabaseFixture.UseMySqlConnection();
        }

        [TestMethod]
        [DoNotParallelize]
        public void DbQuery_SelectTo_Columns()
        {
            SqlGeneratorCache.Initialise();

            var projectedQuery = new DbQuery<Customers>()
                .SelectTo(x => new CustomerName { Name = x.Forename + " " + x.Surname });

            string sql = projectedQuery.Query.GetSqlQuery(DatabaseType.MySql);

            Assert.AreEqual(
                "SELECT `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname` FROM `Customers`",
                sql,
                "SelectTo should only query the specified columns"
            );
        }

        [TestMethod]
        [DoNotParallelize]
        public void DbQuery_GetSql_GetParameters()
        {
            SqlGeneratorCache.Initialise();

            bool active = false;
            var query = new DbQuery<Customers>()
                .Where(x => x.Active == active && x.AddressLine1 == "Test Street")
                .OrderBy(x => x.Forename)
                .ThenByDescending(x => x.Surname);
            
            string sql = query.GetSqlQuery(DatabaseType.MySql);

            Assert.AreEqual(
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE ((`Customers`.`Active` = @p1) AND (`Customers`.`AddressLine1` = @p2)) ORDER BY `Customers`.`Forename` ASC, `Customers`.`Surname` DESC",
                sql,
                "Incorrect SQL generated from select query"
            );

            var parameters = query.GetSqlParameters();
            var expectedParameters = new Dictionary<string, object>
            {
                { "p1", false },
                { "p2", "Test Street" }
            };
            Assert.IsTrue(
                expectedParameters.Count == parameters.Count
                && expectedParameters.Keys.All(key => parameters.ContainsKey(key) && expectedParameters[key].ToString().Equals(parameters[key].ToString())),
                "Incorrect order by inside select query"
            );
        }

        [TestMethod]
        [DoNotParallelize]
        public void DbQuery_GetSql_Pagination()
        {
            SqlGeneratorCache.Initialise();

            var query = new DbQuery<Customers>()
                .Select(x => x.Forename)
                .OrderBy(x => x.Forename)
                .SetPagination(pageNumber: 2, pageSize: 10);

            string sql = query.GetSqlQuery(DatabaseType.MySql);

            Assert.AreEqual(
                "SELECT `Customers`.`Forename` AS `Forename` FROM `Customers` ORDER BY `Customers`.`Forename` ASC LIMIT 10 OFFSET 10",
                sql,
                "Incorrect paginated query"
            );
        } 
    }
}
