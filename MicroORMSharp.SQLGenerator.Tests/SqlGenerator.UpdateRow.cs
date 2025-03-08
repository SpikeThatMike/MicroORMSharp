using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void UpdateRow_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            var data = new Customers
            {
                Forename = "John",
                Surname = "Doe",
                AddressLine1 = "123 Fake Street",
                AddressLine2 = "Fakeville",
                AddressLine3 = "Faketon",
                AddressLine4 = "Fakeshire",
                Postcode = "FA1 2KE",
                Nullable = 1,
                NotNullable = 2
            };

            var createTable = sqlGenerator.UpdateRow(data, false);
            var query = createTable.ToString();

            Assert.AreEqual(
                "UPDATE `Customers` SET `Customers`.`Forename` = @p2, `Customers`.`Surname` = @p3, `Customers`.`AddressLine1` = @p4, `Customers`.`AddressLine2` = @p5, `Customers`.`AddressLine3` = @p6, `Customers`.`AddressLine4` = @p7, `Customers`.`Postalcode` = @p8, `Customers`.`Nullable` = @p9, `Customers`.`NotNullable` = @p10, `Customers`.`Active` = @p11 WHERE `Customers`.`Id` = @p1;",
                query,
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateRow_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            var data = new Customers
            {
                Forename = "John",
                Surname = "Doe",
                AddressLine1 = "123 Fake Street",
                AddressLine2 = "Fakeville",
                AddressLine3 = "Faketon",
                AddressLine4 = "Fakeshire",
                Postcode = "FA1 2KE",
                Nullable = 1,
                NotNullable = 2
            };

            var createTable = sqlGenerator.UpdateRow(data, false);
            var query = createTable.ToString();

            Assert.AreEqual(
                "UPDATE [dbo].[Customers] SET [Customers].[Forename] = @p2, [Customers].[Surname] = @p3, [Customers].[AddressLine1] = @p4, [Customers].[AddressLine2] = @p5, [Customers].[AddressLine3] = @p6, [Customers].[AddressLine4] = @p7, [Customers].[Postalcode] = @p8, [Customers].[Nullable] = @p9, [Customers].[NotNullable] = @p10, [Customers].[Active] = @p11 WHERE [Customers].[Id] = @p1;",
                query,
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateSelectRow_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            var data = new Customers
            {
                Forename = "John",
                Surname = "Doe",
                AddressLine1 = "123 Fake Street",
                AddressLine2 = "Fakeville",
                AddressLine3 = "Faketon",
                AddressLine4 = "Fakeshire",
                Postcode = "FA1 2KE",
                Nullable = 1,
                NotNullable = 2
            };

            var createTable = sqlGenerator.UpdateRow(data, true);
            var query = createTable.ToString();

            Assert.AreEqual(
                "UPDATE `Customers` SET `Customers`.`Forename` = @p2, `Customers`.`Surname` = @p3, `Customers`.`AddressLine1` = @p4, `Customers`.`AddressLine2` = @p5, `Customers`.`AddressLine3` = @p6, `Customers`.`AddressLine4` = @p7, `Customers`.`Postalcode` = @p8, `Customers`.`Nullable` = @p9, `Customers`.`NotNullable` = @p10, `Customers`.`Active` = @p11 WHERE `Customers`.`Id` = @p1; SELECT `Customers`.`Id` AS `Id`, `Customers`.`Forename` AS `Forename`, `Customers`.`Surname` AS `Surname`, `Customers`.`AddressLine1` AS `AddressLine1`, `Customers`.`AddressLine2` AS `AddressLine2`, `Customers`.`AddressLine3` AS `AddressLine3`, `Customers`.`AddressLine4` AS `AddressLine4`, `Customers`.`Postalcode` AS `Postcode`, `Customers`.`Nullable` AS `Nullable`, `Customers`.`NotNullable` AS `NotNullable`, `Customers`.`Active` AS `Active` FROM `Customers` WHERE `Customers`.`Id` = @p1;",
                query,
                "Update row queries do not match"
            );
        }

        [TestMethod]
        public void UpdateSelectRow_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            var data = new Customers
            {
                Forename = "John",
                Surname = "Doe",
                AddressLine1 = "123 Fake Street",
                AddressLine2 = "Fakeville",
                AddressLine3 = "Faketon",
                AddressLine4 = "Fakeshire",
                Postcode = "FA1 2KE",
                Nullable = 1,
                NotNullable = 2
            };

            var createTable = sqlGenerator.UpdateRow(data, true);
            var query = createTable.ToString();

            Assert.AreEqual(
                "UPDATE [dbo].[Customers] SET [Customers].[Forename] = @p2, [Customers].[Surname] = @p3, [Customers].[AddressLine1] = @p4, [Customers].[AddressLine2] = @p5, [Customers].[AddressLine3] = @p6, [Customers].[AddressLine4] = @p7, [Customers].[Postalcode] = @p8, [Customers].[Nullable] = @p9, [Customers].[NotNullable] = @p10, [Customers].[Active] = @p11 WHERE [Customers].[Id] = @p1; SELECT [Customers].[Id] AS [Id], [Customers].[Forename] AS [Forename], [Customers].[Surname] AS [Surname], [Customers].[AddressLine1] AS [AddressLine1], [Customers].[AddressLine2] AS [AddressLine2], [Customers].[AddressLine3] AS [AddressLine3], [Customers].[AddressLine4] AS [AddressLine4], [Customers].[Postalcode] AS [Postcode], [Customers].[Nullable] AS [Nullable], [Customers].[NotNullable] AS [NotNullable], [Customers].[Active] AS [Active] FROM [dbo].[Customers] WHERE [Customers].[Id] = @p1;",
                query,
                "Update row queries do not match"
            );
        }
    }
}
