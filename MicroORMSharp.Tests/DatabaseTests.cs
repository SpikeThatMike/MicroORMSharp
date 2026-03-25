using MicroORMSharp.Tests.Models;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Linq;
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

        [TestMethod]
        [DoNotParallelize]
        public async Task JoinQueries_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = new TestJoinCustomer
            {
                Name = "Join Test Customer",
                Email = "join-test@example.com",
                CreatedDate = new DateTime(2026, 3, 24, 10, 0, 0, DateTimeKind.Utc)
            };

            var orderTemplate = new TestJoinOrder();

            await EnsureTableCreatedAsync(customer);
            await EnsureTableCreatedAsync(orderTemplate);

            try
            {
                customer = await customer.InsertAsync();

                await new TestJoinOrder
                {
                    CustomerId = customer.Id,
                    OrderDate = customer.CreatedDate,
                    TotalAmount = 10.50m,
                    Status = "Open"
                }.InsertAsync();

                await new TestJoinOrder
                {
                    CustomerId = customer.Id,
                    OrderDate = customer.CreatedDate.AddHours(1),
                    TotalAmount = 25.00m,
                    Status = "Paid"
                }.InsertAsync();

                var query = Database.Query<TestJoinCustomer>()
                    .Where(x => x.Id == customer.Id);

                var executeResult = (await query.ExecuteAsync()).FirstOrDefault();
                Assert.AreEqual(2, executeResult.Orders.Count, "Execute should map joined child rows");

                var executeSingleResult = await query.ExecuteSingleAsync();
                Assert.IsNotNull(executeSingleResult, "ExecuteSingle returned null for a joined query");
                Assert.AreEqual(2, executeSingleResult.Orders.Count, "ExecuteSingle should map joined child rows");

                Assert.IsTrue(await query.AnyAsync(), "Any should return true for a joined query with matching rows");
                Assert.AreEqual(1, await query.CountAsync(), "Count should return the number of parent rows after join mapping");
            }
            finally
            {
                await DropTableIfExistsAsync(orderTemplate);
                await DropTableIfExistsAsync(customer);
            }
        }

        private static async Task EnsureTableCreatedAsync<T>(T entity) where T : IMicroORMSharp
        {
            if (!await entity.TableExistsAsync())
            {
                await entity.CreateTableAsync();
            }
        }

        private static async Task DropTableIfExistsAsync<T>(T entity) where T : IMicroORMSharp
        {
            if (await entity.TableExistsAsync())
            {
                await entity.DropTableAsync();
            }
        }
    }
}
