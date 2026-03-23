using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void Take_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().Take(1);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` LIMIT 1",
                "Take queries do not match"
            );
        }

        [TestMethod]
        public void Take_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Take(1);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT TOP (1) [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers]",
                "Take queries do not match"
            );
        }

        [TestMethod]
        public void Select_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().Select(x => x.Forename);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(sqlQuery, "SELECT `Customers`.`Forename` AS `Forename` FROM `Customers`", "Select queries do not match");
        }

        [TestMethod]
        public void Select_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().Select(x => x.Forename);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(sqlQuery, "SELECT [Customers].[Forename] AS [Forename] FROM [dbo].[Customers]", "Select queries do not match");
        }

        [TestMethod]
        public void Where_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>()
                .Where(x => x.Forename == "John" && x.NotNullable > 10 && x.Nullable == null && x.Active && !x.Active);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE (((((`Customers`.`Forename` = @p1) AND (`Customers`.`NotNullable` > 10)) AND (`Customers`.`Nullable` IS NULL)) AND `Customers`.`Active` = 1) AND (NOT (`Customers`.`Active` = 1)))",
                "Where queries do not match"
            );
        }

        [TestMethod]
        public void Where_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>()
                .Where(x => x.Forename == "John" && x.NotNullable > 10 && x.Nullable == null && x.Active && !x.Active);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE ((((([Customers].[Forename] = @p1) AND ([Customers].[NotNullable] > 10)) AND ([Customers].[Nullable] IS NULL)) AND [Customers].[Active] = 1) AND (NOT ([Customers].[Active] = 1)))",
                "Where queries do not match"
            );
        }

        [TestMethod]
        public void OrderBy_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>()
                .OrderBy(x => x.Forename)
                .ThenByDescending(x => x.Surname);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` ORDER BY `Customers`.`Forename` ASC, `Customers`.`Surname` DESC",
                "Order by queries do not match"
            );
        }

        [TestMethod]
        public void OrderBy_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>()
                .OrderBy(x => x.Forename)
                .ThenByDescending(x => x.Surname);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] ORDER BY [Customers].[Forename] ASC, [Customers].[Surname] DESC",
                "Order by queries do not match"
            );
        }

        [TestMethod]
        public void SelectJoin_MySql()
        {
            var sqlGenerator = new SqlGenerator<CustomersJoined>(DatabaseType.MySql);
            var dbQuery = new DbQuery<CustomersJoined>();

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customer`.`Id` AS `Id`, `Customer`.`Name` AS `Name`, `Customer`.`Email` AS `Email`, `Customer`.`CreatedDate` AS `CreatedDate`, `Order`.`Id` AS `Id`, `Order`.`CustomerId` AS `CustomerId`, `Order`.`OrderDate` AS `OrderDate`, `Order`.`TotalAmount` AS `TotalAmount`, `Order`.`Status` AS `Status` FROM `Customer`  INNER JOIN `Order` ON `Order`.`CustomerId` = `Customer`.`Id`",
                "Select queries do not match"
            );
        }
    }
}
