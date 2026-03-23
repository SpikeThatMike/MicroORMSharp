using MicroORMSharp.Tests.Models;
using MicroORMSharp.SqlGenerator;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public sealed class DatabaseTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestDatabaseFixture.EnsureMySqlConnection();
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Execute_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                await customer.InsertAsync();

                var results = await Database.Query<Customers>()
                    .Where(x => x.Forename == customer.Forename)
                    .ExecuteAsync();

                Assert.AreEqual(1, results.Count(), "ExecuteAsync returned an unexpected row count");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task ExecuteSingle_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                customer = await customer.InsertAsync();

                var result = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingleAsync();

                Assert.IsNotNull(result, "ExecuteSingleAsync returned null");
                Assert.AreEqual(customer.Id, result.Id, "ExecuteSingleAsync returned the wrong customer");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task ExecuteJoin_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var results = await Database.Query<CustomersJoined>().ExecuteAsync();
            Assert.IsNotNull(results, "ExecuteAsync join query returned null");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task ExecuteJoinSingle_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            await Database.Query<CustomersJoined>().ExecuteSingleAsync();
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Any_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                await customer.InsertAsync();

                var hasCustomers = await Database.Query<Customers>().AnyAsync();
                Assert.IsTrue(hasCustomers, "AnyAsync should return true when rows exist");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Count_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomerBatch();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                await customers.InsertAsync();

                var count = await Database.Query<Customers>().CountAsync();
                Assert.AreEqual(2, count, "CountAsync returned an unexpected row count");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
