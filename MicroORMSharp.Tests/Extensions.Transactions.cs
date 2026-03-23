using System.Data;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task ExtensionMethods_ReuseTransactionConnection_MySql()
        {
            UseMySqlConnection();

            var customer = CreateCustomer();
            await EnsureTableCreatedAsync(customer);

            using var connection = Database.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                customer = await customer.InsertAsync(dbTransaction: transaction);
                Assert.IsTrue(customer.Id > 0, "InsertAsync should return the inserted identity");

                customer.Forename = "Jane";
                customer = await customer.UpdateAsync(dbTransaction: transaction);
                Assert.AreEqual("Jane", customer.Forename, "UpdateAsync should run inside the transaction");

                var exists = await customer.TableExistsAsync(dbTransaction: transaction);
                Assert.IsTrue(exists, "TableExistsAsync should use the same transaction connection");

                await customer.DeleteAsync(dbTransaction: transaction);

                var countInTransaction = await Database.Dapper.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Customers WHERE Id = @Id;",
                    new { customer.Id },
                    transaction: transaction
                );

                Assert.AreEqual(0, countInTransaction, "DeleteAsync should affect the same transaction-scoped connection");

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
        public async Task ExtensionMethods_UseExplicitConnection_MySql()
        {
            UseMySqlConnection();

            var customer = CreateCustomer();
            await EnsureTableCreatedAsync(customer);

            using var connection = Database.GetConnection();
            connection.Open();

            try
            {
                customer = await customer.InsertAsync(dbConnection: connection);
                Assert.IsTrue(customer.Id > 0, "InsertAsync should use the caller-provided connection");

                var exists = await customer.TableExistsAsync(dbConnection: connection);
                Assert.IsTrue(exists, "TableExistsAsync should use the caller-provided connection");
            }
            finally
            {
                connection.Close();
                await AssertTableDroppedAsync(customer);
            }
        }
    }
}
