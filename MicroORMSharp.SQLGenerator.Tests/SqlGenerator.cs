using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    [TestClass]
    public sealed partial class SqlGenerator
    {
        private static SqlGenerator<Customers> CreateCustomerGenerator(DatabaseType dbType)
        {
            return new SqlGenerator<Customers>(dbType);
        }

        private static Customers CreateCustomer()
        {
            return new Customers
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
        }

        private static void AssertQuery(SqlQuery sqlQuery, string expectedSql, string message)
        {
            Assert.AreEqual(expectedSql, sqlQuery.ToString(), message);
        }

        [TestMethod]
        public void Setup()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);

            Assert.AreEqual("[dbo].[Customers]", sqlGenerator.GetFullTableName());
            Assert.AreEqual(1, sqlGenerator.IgnoreProperties.Count());
            Assert.AreEqual("FullName", sqlGenerator.IgnoreProperties.First().Name);
        }
    }
}
