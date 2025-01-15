using MicroORMSharp.SqlGenerator.Tests.Models;
using System.Linq.Expressions;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void Take_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>().Take(1);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT `Id` AS `Id`, `Forename` AS `Forename`, `Surname` AS `Surname`, `AddressLine1` AS `AddressLine1`, `AddressLine2` AS `AddressLine2`, `AddressLine3` AS `AddressLine3`, `AddressLine4` AS `AddressLine4`, `Postalcode` AS `Postcode`, `Nullable` AS `Nullable`, `NotNullable` AS `NotNullable`, `Active` AS `Active` FROM `Customers` LIMIT 1",
                "Take queries do not match"
            );
        }

        [TestMethod]
        public void Take_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>().Take(1);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT TOP (1) [Id] AS [Id], [Forename] AS [Forename], [Surname] AS [Surname], [AddressLine1] AS [AddressLine1], [AddressLine2] AS [AddressLine2], [AddressLine3] AS [AddressLine3], [AddressLine4] AS [AddressLine4], [Postalcode] AS [Postcode], [Nullable] AS [Nullable], [NotNullable] AS [NotNullable], [Active] AS [Active] FROM [dbo].[Customers]",
                "Take queries do not match"
            );
        }

        [TestMethod]
        public void Select_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>().Select(x => x.Forename);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT `Forename` AS `Forename` FROM `Customers`",
                "Select queries do not match"
            );
        }

        [TestMethod]
        public void Select_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>().Select(x => x.Forename);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT [Forename] AS [Forename] FROM [dbo].[Customers]",
                "Select queries do not match"
            );
        }

        [TestMethod]
        public void Where_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>().Where(x => x.Forename == "John" && x.NotNullable > 10 && x.Nullable == null && x.Active && !x.Active);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT `Id` AS `Id`, `Forename` AS `Forename`, `Surname` AS `Surname`, `AddressLine1` AS `AddressLine1`, `AddressLine2` AS `AddressLine2`, `AddressLine3` AS `AddressLine3`, `AddressLine4` AS `AddressLine4`, `Postalcode` AS `Postcode`, `Nullable` AS `Nullable`, `NotNullable` AS `NotNullable`, `Active` AS `Active` FROM `Customers` WHERE (((((`Customers`.`Forename` = @p1) AND (`Customers`.`NotNullable` > 10)) AND (`Customers`.`Nullable` IS NULL)) AND `Customers`.`Active` = 1) AND (NOT (`Customers`.`Active` = 1)))",
                "Where queries do not match"
            );
        }

        [TestMethod]
        public void Where_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>().Where(x => x.Forename == "John" && x.NotNullable > 10 && x.Nullable == null && x.Active && !x.Active);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT [Id] AS [Id], [Forename] AS [Forename], [Surname] AS [Surname], [AddressLine1] AS [AddressLine1], [AddressLine2] AS [AddressLine2], [AddressLine3] AS [AddressLine3], [AddressLine4] AS [AddressLine4], [Postalcode] AS [Postcode], [Nullable] AS [Nullable], [NotNullable] AS [NotNullable], [Active] AS [Active] FROM [dbo].[Customers] WHERE ((((([Customers].[Forename] = @p1) AND ([Customers].[NotNullable] > 10)) AND ([Customers].[Nullable] IS NULL)) AND [Customers].[Active] = 1) AND (NOT ([Customers].[Active] = 1)))",
                "Where queries do not match"
            );
        }

        [TestMethod]
        public void OrderBy_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>()
                .OrderBy(x => x.Forename).ThenByDescending(x => x.Surname);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT `Id` AS `Id`, `Forename` AS `Forename`, `Surname` AS `Surname`, `AddressLine1` AS `AddressLine1`, `AddressLine2` AS `AddressLine2`, `AddressLine3` AS `AddressLine3`, `AddressLine4` AS `AddressLine4`, `Postalcode` AS `Postcode`, `Nullable` AS `Nullable`, `NotNullable` AS `NotNullable`, `Active` AS `Active` FROM `Customers` ORDER BY `Forename` ASC, `Surname` DESC",
                "Order by queries do not match"
            );
        }

        [TestMethod]
        public void OrderBy_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            DbQuery<Customers> dbQuery = new DbQuery<Customers>()
                .OrderBy(x => x.Forename).ThenByDescending(x => x.Surname);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            var query = sqlQuery.ToString();

            Assert.AreEqual(
                query,
                "SELECT [Id] AS [Id], [Forename] AS [Forename], [Surname] AS [Surname], [AddressLine1] AS [AddressLine1], [AddressLine2] AS [AddressLine2], [AddressLine3] AS [AddressLine3], [AddressLine4] AS [AddressLine4], [Postalcode] AS [Postcode], [Nullable] AS [Nullable], [NotNullable] AS [NotNullable], [Active] AS [Active] FROM [dbo].[Customers] ORDER BY [Forename] ASC, [Surname] DESC",
                "Order by queries do not match"
            );
        }
    }
}
