using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void InsertRow_MySql()
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

            var createTable = sqlGenerator.InsertRow(data);
            var query = createTable.ToString();

            Assert.AreEqual(
                query,
                "INSERT INTO `Customers` (Forename, Surname, AddressLine1, AddressLine2, AddressLine3, AddressLine4, Postalcode, Nullable, NotNullable, Active) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)",
                "Insert row queries do not match"
            );
        }

        [TestMethod]
        public void InsertRow_SqlServer()
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

            var createTable = sqlGenerator.InsertRow(data);
            var query = createTable.ToString();

            Assert.AreEqual(
                query,
                "INSERT INTO [dbo].[Customers] (Forename, Surname, AddressLine1, AddressLine2, AddressLine3, AddressLine4, Postalcode, Nullable, NotNullable, Active) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)",
                "Insert row queries do not match"
            );
        }
    }
}
