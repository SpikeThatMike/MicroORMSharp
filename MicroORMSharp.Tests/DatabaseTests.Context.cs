using MicroORMSharp.SqlGenerator;
using MicroORMSharp.Tests.Helpers;

namespace MicroORMSharp.Tests
{
    public partial class DatabaseTests
    {

        [TestMethod]
        public void CreateContext_AssignsNamedConnectionMetadata()
        {
            using var context = Database.CreateContext(TestDatabaseFixture.MySqlReference);
            var query = context.Query<Customers>();

            Assert.AreEqual(DatabaseType.MySql, context.DatabaseType, "Database type doesnt match");
            Assert.AreEqual(DatabaseType.MySql, query._databaseType, "Database type doesnt match");
            Assert.AreSame(context._connection, query._dbConnection, "Connection doesnt match");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task DbQueryMethods_UseContextConnection()
        {
            TestDatabaseFixture.UseMySqlConnection();

            using var context = Database.CreateContext(TestDatabaseFixture.MySqlReference);
            var customer = TestDatabaseFixture.CreateCustomer();

            try
            {
                if (!await context.TableExistsAsync(customer))
                {
                    await context.CreateTableAsync(customer);
                }

                customer = await context.InsertAsync(customer);

                var query = context.Query<Customers>()
                    .Where(x => x.Id == customer.Id);

                var result = await query.ExecuteSingleAsync();

                Assert.IsNotNull(result, "Row not found");
                Assert.AreEqual(customer.Id, result.Id, "Incorrect row returned");
                Assert.IsTrue(await query.AnyAsync(), "AnyAsync doesnt use context connection");
                Assert.AreEqual(1, await query.CountAsync(), "CountAsync doesnt use context connection");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task CreateContext_UsesNamedConnection()
        {
            TestDatabaseFixture.UseMySqlConnection();

            using var context = Database.CreateContext(TestDatabaseFixture.MySqlReference);
            var customer = TestDatabaseFixture.CreateCustomer();

            try
            {
                if (!await context.TableExistsAsync(customer))
                {
                    await context.CreateTableAsync(customer);
                }

                customer = await context.InsertAsync(customer);

                var result = await context.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingleAsync();

                var count = await context.Dapper.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Customers WHERE Id = @Id;",
                    new { customer.Id }
                );

                Assert.IsNotNull(result, "Context query did not return the inserted customer");
                Assert.AreEqual(customer.Id, result.Id, "Context query returned the wrong customer");
                Assert.AreEqual(1, count, "Context Dapper wrapper should reuse the context connection");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);        
            }
        }
    }
}
