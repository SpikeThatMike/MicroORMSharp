using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task Insert_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            await EnsureTableCreatedAsync(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                await AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task InsertAsync_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            await EnsureTableCreatedAsync(customers);

            try
            {
                customers = await customers.InsertAsync();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                await AssertTableDroppedAsync(customers);
            }
        }
    }
}
