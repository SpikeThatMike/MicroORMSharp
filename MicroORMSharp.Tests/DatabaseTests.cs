using MicroORMSharp.Tests.Models;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public partial class DatabaseTests
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
        public async Task DbQueryMethods_UseExplicitConnection_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            using var connection = Database.GetConnection();
            connection.Open();

            try
            {
                customer = await customer.InsertAsync(dbConnection: connection);

                var query = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .SetConnection(connection);

                var result = await query.ExecuteSingleAsync();

                Assert.IsNotNull(result, "Row not found");
                Assert.AreEqual(customer.Id, result.Id, "Incorrect row returned");
                Assert.IsTrue(await query.AnyAsync(), "AnyAsync doesnt use connection");
                Assert.AreEqual(1, await query.CountAsync(), "CountAsync doesnt use connection");
            }
            finally
            {
                connection.Close();
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task DbQueryMethods_ReuseTransactionConnection_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            using var connection = Database.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                customer = await customer.InsertAsync(dbTransaction: transaction);

                var transactionQuery = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .SetTransaction(transaction);

                var transactionResult = await transactionQuery.ExecuteSingleAsync();

                Assert.IsNotNull(transactionResult, "Row not found");
                Assert.AreEqual(customer.Id, transactionResult.Id, "Incorrect row returned");
                Assert.IsTrue(await transactionQuery.AnyAsync(), "AnyAsync doesnt use transaction");
                Assert.AreEqual(1, await transactionQuery.CountAsync(), "CountAsync doesnt use transaction");

                var outsideTransaction = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .AnyAsync();

                var outsideTransactionQuery = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id);

                Assert.IsFalse(outsideTransaction, "A query without SetTransaction should not see the uncommitted row");
                Assert.IsFalse(await outsideTransactionQuery.AnyAsync(), "AnyAsync uses transaction");
                Assert.AreEqual(0, await outsideTransactionQuery.CountAsync(), "CountAsync uses transaction");

                transaction.Rollback();
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
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
