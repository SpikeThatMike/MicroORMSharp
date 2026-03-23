using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task CreateAndDeleteTable_MySql()
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
        public async Task TruncateTable_MySql()
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
