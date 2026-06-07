using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public void CreateAndDeleteTable()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = new Customers();

                TestDatabaseFixture.EnsureTableDropped(customers);

            try
            {
                customers.CreateTable();
                var isCreated = customers.TableExists();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
            finally
            {
                TestDatabaseFixture.EnsureTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task CreateAndDeleteTableAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = new Customers();

            await TestDatabaseFixture.EnsureTableDroppedAsync(customers);

            try
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
            finally
            {
                await TestDatabaseFixture.EnsureTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void TruncateTable()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

                customers.TruncateTable();
                var anyCustomers = Database.Query<Customers>().Any();

                Assert.IsFalse(anyCustomers, "Failed to truncate data from table");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task TruncateTableAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                customers = await customers.InsertAsync();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

                await customers.TruncateTableAsync();
                var anyCustomers = await Database.Query<Customers>().AnyAsync();

                Assert.IsFalse(anyCustomers, "Failed to truncate data from table");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
