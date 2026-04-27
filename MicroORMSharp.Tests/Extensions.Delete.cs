using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public void DeleteRow_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            EnsureTableCreated(customers);

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
                AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task DeleteRowAsync_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            await EnsureTableCreatedAsync(customers);

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
                await AssertTableDroppedAsync(customers);
            }
        }
    }
}
