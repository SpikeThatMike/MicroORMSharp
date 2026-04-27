using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public void CreateAndDeleteTable_MySql()
        {
            UseMySqlConnection();

            var customers = new Customers();

            if (customers.TableExists())
                AssertTableDropped(customers);

            try
            {
                customers.CreateTable();
                var isCreated = customers.TableExists();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
            finally
            {
                if (customers.TableExists())
                    AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task CreateAndDeleteTableAsync_MySql()
        {
            UseMySqlConnection();

            var customers = new Customers();

            if (await customers.TableExistsAsync())
                await AssertTableDroppedAsync(customers);

            try
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
            finally
            {
                if (await customers.TableExistsAsync())
                    await AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void TruncateTable_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            EnsureTableCreated(customers);

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
                AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task TruncateTableAsync_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            await EnsureTableCreatedAsync(customers);

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
                await AssertTableDroppedAsync(customers);
            }
        }
    }
}
