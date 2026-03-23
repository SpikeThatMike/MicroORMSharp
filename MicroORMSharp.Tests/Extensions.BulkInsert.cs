using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task BulkInsert_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomerBatch();
            await EnsureTableCreatedAsync(customers);

            try
            {
                customers.Insert();

                var data = await Database.Query<Customers>().ExecuteAsync();
                Assert.AreEqual(2, data.Count(), "Failed to bulk insert data");
            }
            finally
            {
                await AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task BulkInsertAsync_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomerBatch();
            await EnsureTableCreatedAsync(customers);

            try
            {
                await customers.InsertAsync();

                var data = await Database.Query<Customers>().ExecuteAsync();
                Assert.AreEqual(2, data.Count(), "Failed to bulk insert data");
            }
            finally
            {
                await AssertTableDroppedAsync(customers);
            }
        }
    }
}
