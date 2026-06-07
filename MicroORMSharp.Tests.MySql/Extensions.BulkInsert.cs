using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task BulkInsert()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomerBatch();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                customers.Insert();

                var data = await Database.Query<Customers>().ExecuteAsync();
                Assert.AreEqual(2, data.Count(), "Failed to bulk insert data");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task BulkInsertAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomerBatch();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                await customers.InsertAsync();

                var data = await Database.Query<Customers>().ExecuteAsync();
                Assert.AreEqual(2, data.Count(), "Failed to bulk insert data");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
