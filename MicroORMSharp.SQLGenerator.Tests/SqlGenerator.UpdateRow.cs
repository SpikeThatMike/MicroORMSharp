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

            var createTable = sqlGenerator.UpdateRow(data);
            var query = createTable.ToString();

            Assert.AreEqual(
                query,
                "UPDATE `Customers` SET Forename = @p2, Surname = @p3, AddressLine1 = @p4, AddressLine2 = @p5, AddressLine3 = @p6, AddressLine4 = @p7, Postalcode = @p8, Nullable = @p9, NotNullable = @p10, Active = @p11 WHERE Id = @p1",
                "Delete row queries do not match"
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

            var createTable = sqlGenerator.UpdateRow(data);
            var query = createTable.ToString();

            Assert.AreEqual(
                query,
                "UPDATE [dbo].[Customers] SET Forename = @p2, Surname = @p3, AddressLine1 = @p4, AddressLine2 = @p5, AddressLine3 = @p6, AddressLine4 = @p7, Postalcode = @p8, Nullable = @p9, NotNullable = @p10, Active = @p11 WHERE Id = @p1",
                "Update row queries do not match"
            );
        }
    }
}
