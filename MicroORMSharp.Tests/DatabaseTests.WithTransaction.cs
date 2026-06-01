using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroORMSharp.SqlGenerator;

namespace MicroORMSharp.Tests
{
    public partial class DatabaseTests
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task WithTransaction_Commited_MySql()
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

                var customerCountBefore = await context.Query<Customers>()
                    .CountAsync();

                var result = context.WithTransaction(trans =>
                {
                    customer = trans.Insert(customer);
                });

                var customerCountAfter = await context.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore + 1, customerCountAfter, "Customer wasnt inserted");
                Assert.IsTrue(result, "Transaction wasnt successful");
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
        public async Task WithTransaction_Rollback_MySql()
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

                var customerCountBefore = await context.Query<Customers>()
                    .CountAsync();

                var result = context.WithTransaction(trans =>
                {
                    customer = trans.Insert(customer);
                    throw new Exception("Force rollback");
                });

                var customerCountAfter = await context.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore, customerCountAfter, "Customer was inserted");
                Assert.IsFalse(result, "Transaction was successful");
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
        public async Task WithTransactionAsync_Commited_MySql()
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

                var customerCountBefore = await context.Query<Customers>()
                    .CountAsync();

                var result = await context.WithTransactionAsync(async trans =>
                {
                    customer = await trans.InsertAsync(customer);
                });

                var customerCountAfter = await context.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore + 1, customerCountAfter, "Customer wasnt inserted");
                Assert.IsTrue(result, "Transaction wasnt successful");
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
        public async Task WithTransactionAsync_Rollback_MySql()
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

                var customerCountBefore = await context.Query<Customers>()
                    .CountAsync();

                var result = await context.WithTransactionAsync(async trans =>
                {
                    customer = await trans.InsertAsync(customer);
                    throw new Exception("Force rollback");
                });

                var customerCountAfter = await context.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore, customerCountAfter, "Customer was inserted");
                Assert.IsFalse(result, "Transaction was successful");
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
        public async Task WithTransactionAsync_IsTransactionScoped_MySql()
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

                var result = await context.WithTransactionAsync(async trans =>
                {
                    customer = await trans.InsertAsync(customer);

                    var transactionQuery = trans.Query<Customers>()
                        .Where(x => x.Id == customer.Id);

                    var transactionResult = await transactionQuery.ExecuteSingleAsync();
                    var dapperCount = await trans.Dapper.QuerySingleAsync<int>(
                        "SELECT COUNT(*) FROM Customers WHERE Id = @Id;",
                        new { customer.Id }
                    );

                    Assert.IsNotNull(transactionResult, "Row not found");
                    Assert.AreEqual(customer.Id, transactionResult.Id, "Incorrect row returned");
                    Assert.AreEqual(1, dapperCount, "Transaction Dapper wrapper should use the transaction without explicit parameters");
                    Assert.IsTrue(await transactionQuery.AnyAsync(), "AnyAsync doesnt use transaction");
                    Assert.AreEqual(1, await transactionQuery.CountAsync(), "CountAsync doesnt use transaction");

                    using DBContext dBContext = new DBContext();
                    var outsideTransactionContext = await dBContext.Query<Customers>()
                        .Where(x => x.Id == customer.Id)
                        .AnyAsync();
                    Assert.IsFalse(outsideTransactionContext, "A query without the context transaction should not see the uncommitted row");

                    var outsideTransactionQuery = Database.Query<Customers>()
                        .Where(x => x.Id == customer.Id);

                    Assert.IsFalse(await outsideTransactionQuery.AnyAsync(), "AnyAsync uses transaction");
                    Assert.AreEqual(0, await outsideTransactionQuery.CountAsync(), "CountAsync uses transaction");

                    throw new InvalidOperationException("Force rollback");
                });

                Assert.IsFalse(result, "Transaction should have rolled back");
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
        public async Task WithTransactionAsync_ReuseExtensionMethods_MySql()
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

                var transactionSucceeded = await context.WithTransactionAsync(async trans =>
                {
                    customer = await trans.InsertAsync(customer);
                    Assert.IsTrue(customer.Id > 0, "InsertAsync should return the inserted identity");

                    customer.Forename = "Jane";
                    customer = await trans.UpdateAsync(customer);
                    Assert.AreEqual("Jane", customer.Forename, "UpdateAsync should run inside the transaction");

                    var exists = await trans.TableExistsAsync(customer);
                    Assert.IsTrue(exists, "TableExistsAsync should use the same transaction connection");

                    await trans.DeleteAsync(customer);

                    var countInTransaction = trans.Query<Customers>()
                        .Where(x => x.Id == customer.Id);

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
    }
}
