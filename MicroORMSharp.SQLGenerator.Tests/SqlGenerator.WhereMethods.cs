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
    }
}
