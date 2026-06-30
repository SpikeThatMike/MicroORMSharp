using MicroORMSharp.SqlGenerator;
using MicroORMSharp.Tests.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task BulkInsert(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

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
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task BulkInsertAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

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
