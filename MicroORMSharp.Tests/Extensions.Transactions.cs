using System.Data;
using System.Threading.Tasks;
using MicroORMSharp.SqlGenerator;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task InternalExtensionMethods_ReuseTransactionConnection_MySql()
        {
            UseMySqlConnection();

            using var context = Database.CreateContext(TestDatabaseFixture.MySqlReference);
            var customer = CreateCustomer();

            try
            {
                if (!await context.TableExistsAsync(customer))
                {
                    await context.CreateTableAsync(customer);
                }

                var transactionSucceeded = await context.WithTransactionAsync(async transaction =>
                {
                    customer = await customer.InsertAsync(null, null, context.DatabaseType, context._connection, transaction);
                    Assert.IsTrue(customer.Id > 0, "InsertAsync should return the inserted identity");

                    customer.Forename = "Jane";
                    customer = await customer.UpdateAsync(null, null, context.DatabaseType, context._connection, transaction);
                    Assert.AreEqual("Jane", customer.Forename, "UpdateAsync should run inside the transaction");

                    var exists = await customer.TableExistsAsync(null, null, context.DatabaseType, context._connection, transaction);
                    Assert.IsTrue(exists, "TableExistsAsync should use the same transaction connection");

                    await customer.DeleteAsync(null, null, context.DatabaseType, context._connection, transaction);

                    var countInTransaction = context.Query<Customers>()
                        .Where(x => x.Id == customer.Id);
                    countInTransaction._dbTransaction = transaction;

                    Assert.AreEqual(0, await countInTransaction.CountAsync(), "DeleteAsync should affect the same transaction-scoped connection");
                });

                Assert.IsTrue(transactionSucceeded, "Transaction should commit successfully");
            }
            finally
            {
                if (await context.TableExistsAsync(customer))
                {
                    await context.DropTableAsync(customer);
                }
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task ContextMethods_UseContextConnection_MySql()
        {
            UseMySqlConnection();

            using var context = Database.CreateContext(TestDatabaseFixture.MySqlReference);
            var customer = CreateCustomer();

            try
            {
                if (!await context.TableExistsAsync(customer))
                {
                    await context.CreateTableAsync(customer);
                }

                customer = await context.InsertAsync(customer);
                Assert.IsTrue(customer.Id > 0, "InsertAsync should use the context connection");

                var exists = await context.TableExistsAsync(customer);
                Assert.IsTrue(exists, "TableExistsAsync should use the context connection");
            }
            finally
            {
                if (await context.TableExistsAsync(customer))
                {
                    await context.DropTableAsync(customer);
                }
            }
        }
    }
}
