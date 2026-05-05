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

                var result = context.WithTransaction(transaction =>
                {
                    customer.Insert(null, null, context.DatabaseType, context._connection, transaction);
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

                var result = context.WithTransaction(transaction =>
                {
                    customer.Insert(null, null, context.DatabaseType, context._connection, transaction);
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

                var result = await context.WithTransactionAsync(async transaction =>
                {
                    await customer.InsertAsync(null, null, context.DatabaseType, context._connection, transaction);
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

                var result = await context.WithTransactionAsync(async transaction =>
                {
                    await customer.InsertAsync(null, null, context.DatabaseType, context._connection, transaction);
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
    }
}
