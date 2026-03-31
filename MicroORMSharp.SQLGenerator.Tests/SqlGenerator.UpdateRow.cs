namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void UpdateRow_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.UpdateRow(CreateCustomer(), false);

            AssertQuery(
                sqlQuery,
                "UPDATE `Customers` SET `Customers`.`Forename` = @p2, `Customers`.`Surname` = @p3, `Customers`.`AddressLine1` = @p4, `Customers`.`AddressLine2` = @p5, `Customers`.`AddressLine3` = @p6, `Customers`.`AddressLine4` = @p7, `Customers`.`Postalcode` = @p8, `Customers`.`Nullable` = @p9, `Customers`.`NotNullable` = @p10, `Customers`.`Active` = @p11 WHERE `Customers`.`Id` = @p1;",
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateRow_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.UpdateRow(CreateCustomer(), false);

            AssertQuery(
                sqlQuery,
                "UPDATE [dbo].[Customers] SET [Customers].[Forename] = @p2, [Customers].[Surname] = @p3, [Customers].[AddressLine1] = @p4, [Customers].[AddressLine2] = @p5, [Customers].[AddressLine3] = @p6, [Customers].[AddressLine4] = @p7, [Customers].[Postalcode] = @p8, [Customers].[Nullable] = @p9, [Customers].[NotNullable] = @p10, [Customers].[Active] = @p11 WHERE [Customers].[Id] = @p1;",
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateSelectRow_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.UpdateRow(CreateCustomer(), true);

            AssertQuery(
                sqlQuery,
                "UPDATE `Customers` SET `Customers`.`Forename` = @p2, `Customers`.`Surname` = @p3, `Customers`.`AddressLine1` = @p4, `Customers`.`AddressLine2` = @p5, `Customers`.`AddressLine3` = @p6, `Customers`.`AddressLine4` = @p7, `Customers`.`Postalcode` = @p8, `Customers`.`Nullable` = @p9, `Customers`.`NotNullable` = @p10, `Customers`.`Active` = @p11 WHERE `Customers`.`Id` = @p1; SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE `Customers`.`Id` = @p1;",
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateSelectRow_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.UpdateRow(CreateCustomer(), true);

            AssertQuery(
                sqlQuery,
                "UPDATE [dbo].[Customers] SET [Customers].[Forename] = @p2, [Customers].[Surname] = @p3, [Customers].[AddressLine1] = @p4, [Customers].[AddressLine2] = @p5, [Customers].[AddressLine3] = @p6, [Customers].[AddressLine4] = @p7, [Customers].[Postalcode] = @p8, [Customers].[Nullable] = @p9, [Customers].[NotNullable] = @p10, [Customers].[Active] = @p11 WHERE [Customers].[Id] = @p1; SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE [Customers].[Id] = @p1;",
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateRow_SelectedColumns_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.UpdateRow(CreateCustomer(), x => new { x.Forename, x.Postcode }, false);

            AssertQuery(
                sqlQuery,
                "UPDATE `Customers` SET `Customers`.`Forename` = @p2, `Customers`.`Postalcode` = @p3 WHERE `Customers`.`Id` = @p1;",
                "Partial update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateSelectRow_SelectedColumns_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.UpdateRow(CreateCustomer(), x => new { x.Forename, x.Postcode }, true);

            AssertQuery(
                sqlQuery,
                "UPDATE [dbo].[Customers] SET [Customers].[Forename] = @p2, [Customers].[Postalcode] = @p3 WHERE [Customers].[Id] = @p1; SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE [Customers].[Id] = @p1;",
                "Partial update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateRow_SelectedColumns_RejectsIdentity()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);

            var ex = Assert.ThrowsException<InvalidOperationException>(() => sqlGenerator.UpdateRow(CreateCustomer(), x => new { x.Id, x.Forename }, false));

            StringAssert.Contains(ex.Message, "Invalid selections: Id");
        }
    }
}
