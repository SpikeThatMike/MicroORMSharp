using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public void Insert()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task InsertAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                customers = await customers.InsertAsync();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task InsertOnly()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                var beforeCount = Database.Query<Customers>().Count();
                customers.InsertOnly();
                var afterCount = Database.Query<Customers>().Count();
                Assert.AreEqual(beforeCount + 1, afterCount, "Insert failed");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task InsertOnlyAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                var beforeCount = await Database.Query<Customers>().CountAsync();
                await customers.InsertOnlyAsync();
                var afterCount = await Database.Query<Customers>().CountAsync();
                Assert.AreEqual(beforeCount + 1, afterCount, "Insert failed");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
