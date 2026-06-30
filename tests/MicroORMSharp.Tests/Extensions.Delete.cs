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
        public void DeleteRow(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

                customers.Delete();

                var query = Database.Query<Customers>().Execute();
                Assert.AreEqual(0, query.Count(), "Failed to delete row");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task DeleteRowAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                customers = await customers.InsertAsync();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

                await customers.DeleteAsync();

                var query = await Database.Query<Customers>().ExecuteAsync();
                Assert.AreEqual(0, query.Count(), "Failed to delete row");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
