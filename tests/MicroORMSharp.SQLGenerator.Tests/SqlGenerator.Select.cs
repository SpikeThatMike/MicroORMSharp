using MicroORMSharp.SqlGenerator.Attributes;
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
                "SELECT `Customer`.`Id` AS `Id`, `Customer`.`Name` AS `Name`, `Customer`.`Email` AS `Email`, `Customer`.`CreatedDate` AS `CreatedDate`, `Order`.`Id` AS `Id`, `Order`.`CustomerId` AS `CustomerId`, `Order`.`OrderDate` AS `OrderDate`, `Order`.`TotalAmount` AS `TotalAmount`, `Order`.`Status` AS `Status` FROM `Customer` LEFT JOIN `Order` ON `Order`.`CustomerId` = `Customer`.`Id`",
                "Select queries do not match"
            );
        }

        [TestMethod]
        public void SelectNestedJoin_MySql()
        {
            var sqlGenerator = new SqlGenerator<NestedJoinCustomer>(DatabaseType.MySql);
            var dbQuery = new DbQuery<NestedJoinCustomer>();

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `NestedCustomer`.`Id` AS `Id`, `NestedCustomer`.`Name` AS `Name`, `NestedOrder`.`Id` AS `Id`, `NestedOrder`.`CustomerId` AS `CustomerId`, `NestedOrder`.`StatusId` AS `StatusId`, `NestedOrder`.`OrderDate` AS `OrderDate`, `NestedOrder`.`TotalAmount` AS `TotalAmount`, `NestedOrderStatus`.`Id` AS `Id`, `NestedOrderStatus`.`Name` AS `Name` FROM `NestedCustomer` LEFT JOIN `NestedOrder` ON `NestedOrder`.`CustomerId` = `NestedCustomer`.`Id` INNER JOIN `NestedOrderStatus` ON `NestedOrderStatus`.`Id` = `NestedOrder`.`StatusId`",
                "Nested join queries do not match"
            );
        }

        [TestMethod]
        public void SelectJoinTypes_MySql()
        {
            var sqlGenerator = new SqlGenerator<JoinTypeCustomer>(DatabaseType.MySql);
            var dbQuery = new DbQuery<JoinTypeCustomer>();

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `JoinTypeCustomer`.`Id` AS `Id`, `JoinTypeCustomer`.`Name` AS `Name`, `JoinTypeOrder`.`Id` AS `Id`, `JoinTypeOrder`.`CustomerId` AS `CustomerId`, `JoinTypeOrder`.`StatusId` AS `StatusId`, `JoinTypeStatus`.`Id` AS `Id`, `JoinTypeStatus`.`Name` AS `Name` FROM `JoinTypeCustomer` LEFT JOIN `JoinTypeOrder` ON `JoinTypeOrder`.`CustomerId` = `JoinTypeCustomer`.`Id` INNER JOIN `JoinTypeStatus` ON `JoinTypeStatus`.`Id` = `JoinTypeOrder`.`StatusId`",
                "Join type queries do not match"
            );
        }

        [TestMethod]
        public void SelectNestedJoin_TooDeep_Throws()
        {
            var sqlGenerator = new SqlGenerator<DeepJoinLevel1>(DatabaseType.MySql);
            var dbQuery = new DbQuery<DeepJoinLevel1>();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => sqlGenerator.Select(dbQuery));
            Assert.AreEqual($"Nested joins are limited to {DBJoin.MaxDepth} levels.", ex.Message, "Unexpected nested join depth message");
        }

        [TestMethod]
        public void SetPagination_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>().SetPagination(pageNumber: 2, pageSize: 10);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` LIMIT 10 OFFSET 10",
                "Pagination queries do not match"
            );
        }

        [TestMethod]
        public void SetPagination_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>().SetPagination(pageNumber: 2, pageSize: 10);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] ORDER BY [Customers].[Id] ASC OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY",
                "Pagination queries do not match"
            );
        }

        [TestMethod]
        public void SetPagination_SqlServer_UsesExplicitOrdering()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>()
                .OrderByDescending(x => x.Forename)
                .SetPagination(pageNumber: 3, pageSize: 5);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] ORDER BY [Customers].[Forename] DESC OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY",
                "Pagination queries do not match"
            );
        }

        [TestMethod]
        public void OrderBy_ValueType_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var dbQuery = new DbQuery<Customers>()
                .OrderBy(x => x.Id);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` ORDER BY `Customers`.`Id` ASC",
                "Order by queries do not match"
            );
        }

        [TestMethod]
        public void OrderBy_ValueType_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var dbQuery = new DbQuery<Customers>()
                .OrderBy(x => x.Id);

            var sqlQuery = sqlGenerator.Select(dbQuery);
            AssertQuery(
                sqlQuery,
                "SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] ORDER BY [Customers].[Id] ASC",
                "Order by queries do not match"
            );
        }
    }
}
