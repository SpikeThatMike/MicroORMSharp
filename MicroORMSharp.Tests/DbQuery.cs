using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.Tests.Models;
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

        [TestMethod]
        public void DbQuery_SelectTo()
        {
            var projectedQuery = new DbQuery<Customers>()
                .SelectTo(x => new CustomerName { Name = x.Forename + " " + x.Surname });

            var result = projectedQuery.Selector.Compile()(new Customers { Forename = "John", Surname = "Doe" });
            var selectedColumns = projectedQuery.Query._selectClause.Select(x => x.GetCustomAttribute<DbColumn>()?.Name ?? x.Name).ToList();

            Assert.IsNotNull(projectedQuery.Query, "Query did not keep the original query");
            CollectionAssert.AreEqual(new List<string> { "Forename", "Surname" }, selectedColumns, "Query selected the wrong columns");
            Assert.AreEqual("John Doe", result.Name, "Selector returned the wrong value");
        }

        [TestMethod]
        [DoNotParallelize]
        public void DbQuery_SelectTo_Columns_MySql()
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
        public void DbQuery_GetSql_GetParameters_MySql()
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
        public void DbQuery_GetSql_GetParameters_SqlServer()
        {
            SqlGeneratorCache.Initialise();

            bool active = true;
            var query = new DbQuery<Customers>()
                .Where(x => x.Active == active && x.AddressLine1 == "Test Street")
                .OrderBy(x => x.Forename)
                .ThenByDescending(x => x.Surname);

            string sql = query.GetSqlQuery(DatabaseType.SqlServer);

            Assert.AreEqual(
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE (([Customers].[Active] = @p1) AND ([Customers].[AddressLine1] = @p2)) ORDER BY [Customers].[Forename] ASC, [Customers].[Surname] DESC",
                sql,
                "Incorrect SQL generated from select query"
            );

            var parameters = query.GetSqlParameters();
            var expectedParameters = new Dictionary<string, object>
            {
                { "p1", true },
                { "p2", "Test Street" }
            };
            Assert.IsTrue(
                expectedParameters.Count == parameters.Count
                && expectedParameters.Keys.All(key => parameters.ContainsKey(key) && expectedParameters[key].ToString().Equals(parameters[key].ToString())),
                "Incorrect order by inside select query"
            );
        }
    }
}
