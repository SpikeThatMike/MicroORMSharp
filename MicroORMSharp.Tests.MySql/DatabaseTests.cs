using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.Tests.Models;

namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public partial class DatabaseTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
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
        public async Task Execute_SelectTo_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomerBatch();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                string fillingString = " ";
                await customers.InsertAsync();

                var syncResults = Database.Query<Customers>()
                    .SelectTo(x => new CustomerName { Name = x.Forename + fillingString + x.Surname })
                    .Execute()
                    .ToList();

                Assert.AreEqual(2, syncResults.Count, "Execute returned incorrect rows");
                CollectionAssert.AreEqual(
                    customers.OrderBy(x => x.Forename).Select(x => x.Forename + fillingString + x.Surname).ToList(),
                    syncResults.Select(x => x.Name).ToList(),
                    "Execute returned the wrong values"
                );

                var asyncResults = (await Database.Query<Customers>()
                    .SelectTo(x => new CustomerName { Name = x.Forename + fillingString + x.Surname })
                    .ExecuteAsync())
                    .ToList();

                Assert.AreEqual(2, asyncResults.Count, "ExecuteAsync returned incorrect rows");
                CollectionAssert.AreEqual(
                    customers.OrderBy(x => x.Forename).Select(x => x.Forename + fillingString + x.Surname).ToList(),
                    asyncResults.Select(x => x.Name).ToList(),
                    "ExecuteAsync returned the wrong values"
                );
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task ExecuteSingle_SelectTo_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                string fillingString = " ";
                customer = await customer.InsertAsync();

                var syncResult = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .SelectTo(x => new CustomerName { Name = x.Forename + fillingString + x.Surname })
                    .ExecuteSingle();

                Assert.IsNotNull(syncResult, "ExecuteSingle returned null");
                Assert.AreEqual(customer.Forename + fillingString + customer.Surname, syncResult.Name, "ExecuteSingle returned the wrong value");

                var asyncResult = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .SelectTo(x => new CustomerName { Name = x.Forename + fillingString + x.Surname })
                    .ExecuteSingleAsync();

                Assert.IsNotNull(asyncResult, "ExecuteSingleAsync returned null");
                Assert.AreEqual(customer.Forename + fillingString + customer.Surname, asyncResult.Name, "ExecuteSingleAsync returned the wrong value");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task ExecuteSingle_SelectTo_NoRowExists_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                string fillingString = " ";
                var syncResult = Database.Query<Customers>()
                    .Where(x => x.Id == -1)
                    .SelectTo(x => new CustomerName { Name = x.Forename + fillingString + x.Surname })
                    .ExecuteSingle();

                var asyncResult = await Database.Query<Customers>()
                    .Where(x => x.Id == -1)
                    .SelectTo(x => new CustomerName { Name = x.Forename + fillingString + x.Surname })
                    .ExecuteSingleAsync();

                Assert.IsNull(syncResult, "ExecuteSingle should return null when no row exists");
                Assert.IsNull(asyncResult, "ExecuteSingleAsync should return null when no row exists");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
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
        public async Task DbQueryMethods_Pagination_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customers = TestDatabaseFixture.CreateCustomerBatch();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                await customers.InsertAsync();

                var query1st = Database.Query<Customers>()
                    .OrderByDescending(x => x.Id)
                    .SetPagination(2, 1);

                var query2nd = Database.Query<Customers>()
                    .OrderBy(x => x.Id)
                    .SetPagination(2, 1);

                var result1st = await query1st.ExecuteAsync();
                var result2nd = await query2nd.ExecuteAsync();

                Assert.AreEqual(1, result1st.Count(), "Incorrect number of rows for 1st page");
                Assert.AreEqual(1, result2nd.Count(), "Incorrect number of rows for 2nd page");

                Assert.AreEqual(1, result1st.First().Id, "Incorrect row for 1st page");
                Assert.AreEqual(2, result2nd.First().Id, "Incorrect row for 2nd page");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
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
