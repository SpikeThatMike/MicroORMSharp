using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public partial class Extensions
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
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

        private static Task EnsureTableCreatedAsync(Customers customers)
        {
            return TestDatabaseFixture.EnsureTableCreatedAsync(customers);
        }

        private static Task EnsureTableCreatedAsync(List<Customers> customers)
        {
            return TestDatabaseFixture.EnsureTableCreatedAsync(customers);
        }

        private static Task AssertTableDroppedAsync(Customers customers)
        {
            return TestDatabaseFixture.AssertTableDroppedAsync(customers);
        }

        private static Task AssertTableDroppedAsync(List<Customers> customers)
        {
            return TestDatabaseFixture.AssertTableDroppedAsync(customers);
        }
    }
}
