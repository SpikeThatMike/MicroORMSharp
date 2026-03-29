using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public partial class Extensions
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
        }

        private static void UseMySqlConnection()
        {
            TestDatabaseFixture.UseMySqlConnection();
        }

        private static Customers CreateCustomer(string suffix = "")
        {
            return TestDatabaseFixture.CreateCustomer(suffix);
        }

        private static List<Customers> CreateCustomerBatch()
        {
            return TestDatabaseFixture.CreateCustomerBatch();
        }

        private static Task EnsureTableCreatedAsync<T>(T table) where T : IMicroORMSharp
        {
            return TestDatabaseFixture.EnsureTableCreatedAsync(table);
        }

        private static Task EnsureTableCreatedAsync<T>(List<T> customers) where T : IMicroORMSharp
        {
            return TestDatabaseFixture.EnsureTableCreatedAsync(customers);
        }

        private static Task AssertTableDroppedAsync<T>(T customers) where T : IMicroORMSharp
        {
            return TestDatabaseFixture.AssertTableDroppedAsync(customers);
        }

        private static Task AssertTableDroppedAsync<T>(List<T> customers) where T : IMicroORMSharp
        {
            return TestDatabaseFixture.AssertTableDroppedAsync(customers);
        }
    }
}
