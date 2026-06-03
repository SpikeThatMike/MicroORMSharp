using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public void Insert_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            EnsureTableCreated(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                AssertTableDropped(customers);
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

        [TestMethod]
        [DoNotParallelize]
        public async Task InsertOnly_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            EnsureTableCreated(customers);

            try
            {
                var beforeCount = Database.Query<Customers>().Count();
                customers.InsertOnly();
                var afterCount = Database.Query<Customers>().Count();
                Assert.AreEqual(beforeCount + 1, afterCount, "Insert failed");
            }
            finally
            {
                AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task InsertOnlyAsync_MySql()
        {
            UseMySqlConnection();

            var customers = CreateCustomer();
            await EnsureTableCreatedAsync(customers);

            try
            {
                var beforeCount = await Database.Query<Customers>().CountAsync();
                await customers.InsertOnlyAsync();
                var afterCount = await Database.Query<Customers>().CountAsync();
                Assert.AreEqual(beforeCount + 1, afterCount, "Insert failed");
            }
            finally
            {
                await AssertTableDroppedAsync(customers);
            }
        }
    }
}
