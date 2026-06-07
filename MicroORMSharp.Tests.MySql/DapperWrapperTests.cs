using MicroORMSharp.Helpers;
using MicroORMSharp.SqlGenerator;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public sealed class DapperWrapperTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task QueryAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                customer = await customer.InsertAsync();
                Assert.IsTrue(customer.Id > 0, "Failed to retrieve data from insert");

                var results = await Database.Dapper.QueryAsync<string>(
                    $"SELECT `Forename` FROM {Helper.GetTableName<Customers>()};"
                );

                Assert.AreEqual(1, results.Count(), "Incorrect result count");
                Assert.AreEqual(customer.Forename, results.Single(), "Returned name does not match inserted data");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task QueryAsync_UsesTransactionConnection()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            var connection = Database.GetConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                await Database.Dapper.ExecuteAsync(
                    $"INSERT INTO {Helper.GetTableName<Customers>()} (`Forename`, `Surname`, `AddressLine1`, `AddressLine2`, `AddressLine3`, `AddressLine4`, `Postalcode`, `Nullable`, `NotNullable`, `Active`) " +
                    "VALUES (@Forename, @Surname, @AddressLine1, @AddressLine2, @AddressLine3, @AddressLine4, @Postcode, @Nullable, @NotNullable, @Active);",
                    customer,
                    transaction: transaction
                );

                customer.Id = await Database.Dapper.QuerySingleAsync<int>(
                    "SELECT LAST_INSERT_ID();",
                    transaction: transaction
                );

                var countInTransaction = await Database.Dapper.QuerySingleAsync<int>(
                    $"SELECT COUNT(*) FROM {Helper.GetTableName<Customers>()} WHERE `Id` = @Id;",
                    new { customer.Id },
                    transaction: transaction
                );

                Assert.AreEqual(1, countInTransaction, "Expected the transaction-scoped query to use the existing transaction connection");

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();

                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task QueryAsync_UsesExplicitConnection()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            using var connection = Database.GetConnection();
            connection.Open();

            try
            {
                await Database.Dapper.ExecuteAsync(
                    $"INSERT INTO {Helper.GetTableName<Customers>()} (`Forename`, `Surname`, `AddressLine1`, `AddressLine2`, `AddressLine3`, `AddressLine4`, `Postalcode`, `Nullable`, `NotNullable`, `Active`) " +
                    "VALUES (@Forename, @Surname, @AddressLine1, @AddressLine2, @AddressLine3, @AddressLine4, @Postcode, @Nullable, @NotNullable, @Active);",
                    customer,
                    connection: connection
                );

                var count = await Database.Dapper.QuerySingleAsync<int>(
                    $"SELECT COUNT(*) FROM {Helper.GetTableName<Customers>()};",
                    connection: connection
                );

                Assert.AreEqual(1, count, "Expected Dapper to use the caller-provided connection");
            }
            finally
            {
                connection.Close();
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }
    }
}
