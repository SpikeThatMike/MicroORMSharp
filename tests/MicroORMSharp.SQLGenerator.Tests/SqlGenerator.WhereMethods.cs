using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void Where_StringContains_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.Contains("ohn"));

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE (`Customers`.`Forename` LIKE @p1)",
                "Contains where query does not match"
            );

            Assert.AreEqual("%ohn%", sqlQuery.Parameters["p1"], "Contains parameter value does not match");
        }

        [TestMethod]
        public void Where_StringStartsWith_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Surname.StartsWith("Do"));

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE ([Customers].[Surname] LIKE @p1)",
                "StartsWith where query does not match"
            );

            Assert.AreEqual("Do%", sqlQuery.Parameters["p1"], "StartsWith parameter value does not match");
        }

        [TestMethod]
        public void Where_ListContains_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            IEnumerable<int> list = [1];
            var dbQuery = new DbQuery<Customers>().Where(x => list.Contains(x.NotNullable));

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE (`Customers`.`NotNullable` IN (@p1))",
                "Contains where query does not match"
            );
        }

        [TestMethod]
        public void Where_ListContains_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            IEnumerable<int> list = [1];
            var dbQuery = new DbQuery<Customers>().Where(x => list.Contains(x.NotNullable));

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE ([Customers].[NotNullable] IN (@p1))",
                "Contains where query does not match"
            );
        }

        [TestMethod]
        public void Where_StringEndsWith_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Surname.EndsWith("oe"));

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE ([Customers].[Surname] LIKE @p1)",
                "EndsWith where query does not match"
            );

            Assert.AreEqual("%oe", sqlQuery.Parameters["p1"], "EndsWith parameter value does not match");
        }

        [TestMethod]
        public void Where_StringTrim_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.Trim() == "John");

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE (TRIM(`Customers`.`Forename`) = @p1)",
                "Trim where query does not match"
            );

            Assert.AreEqual("John", sqlQuery.Parameters["p1"], "Trim parameter value does not match");
        }

        [TestMethod]
        public void Where_StringTrimStart_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.TrimStart() == "John");

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE (LTRIM(`Customers`.`Forename`) = @p1)",
                "TrimEnd where query does not match"
            );

            Assert.AreEqual("John", sqlQuery.Parameters["p1"], "TrimEnd parameter value does not match");
        }

        [TestMethod]
        public void Where_StringTrimEnd_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.TrimEnd() == "John");

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE (RTRIM(`Customers`.`Forename`) = @p1)",
                "TrimEnd where query does not match"
            );

            Assert.AreEqual("John", sqlQuery.Parameters["p1"], "TrimEnd parameter value does not match");
        }

        [TestMethod]
        public void Where_StringTrim_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.Trim() == "John");

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE (TRIM([Customers].[Forename]) = @p1)",
                "Trim where query does not match"
            );

            Assert.AreEqual("John", sqlQuery.Parameters["p1"], "Trim parameter value does not match");
        }

        [TestMethod]
        public void Where_StringTrimStart_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.TrimStart() == "John");

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE (LTRIM([Customers].[Forename]) = @p1)",
                "TrimStart where query does not match"
            );

            Assert.AreEqual("John", sqlQuery.Parameters["p1"], "TrimStart parameter value does not match");
        }

        [TestMethod]
        public void Where_StringTrimEnd_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Where(x => x.Forename.TrimEnd() == "John");

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE (RTRIM([Customers].[Forename]) = @p1)",
                "TrimStart where query does not match"
            );

            Assert.AreEqual("John", sqlQuery.Parameters["p1"], "TrimStart parameter value does not match");
        }
    }
}
