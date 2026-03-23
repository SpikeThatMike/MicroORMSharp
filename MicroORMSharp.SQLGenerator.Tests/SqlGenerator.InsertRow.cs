namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void InsertRow_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.InsertRow(CreateCustomer());

            AssertQuery(
                sqlQuery,
                "INSERT INTO `Customers` (Forename, Surname, AddressLine1, AddressLine2, AddressLine3, AddressLine4, Postalcode, Nullable, NotNullable, Active) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10); SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE `Customers`.`Id` = (SELECT LAST_INSERT_ID());",
                "Insert row queries do not match"
            );
        }

        [TestMethod]
        public void InsertRow_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.InsertRow(CreateCustomer());

            AssertQuery(
                sqlQuery,
                "INSERT INTO [dbo].[Customers] (Forename, Surname, AddressLine1, AddressLine2, AddressLine3, AddressLine4, Postalcode, Nullable, NotNullable, Active) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10); SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE [Customers].[Id] = (SELECT SCOPE_IDENTITY());",
                "Insert row queries do not match"
            );
        }
    }
}
