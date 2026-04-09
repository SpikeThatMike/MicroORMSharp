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
            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            try
            {
                var customerCountBefore = await Database.Query<Customers>()
                    .CountAsync();

                var result = Database.WithTransaction(transaction =>
                {
                    customer.Insert(dbTransaction: transaction);
                });

                var customerCountAfter = await Database.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore + 1, customerCountAfter, "Customer wasnt inserted");
                Assert.IsTrue(result, "Transaction wasnt successful");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task WithTransaction_Rollback_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();
            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            try
            {
                var customerCountBefore = await Database.Query<Customers>()
                    .CountAsync();

                var result = Database.WithTransaction(transaction =>
                {
                    customer.Insert(dbTransaction: transaction);
                    throw new Exception("Force rollback");
                });

                var customerCountAfter = await Database.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore, customerCountAfter, "Customer was inserted");
                Assert.IsFalse(result, "Transaction was successful");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task WithTransactionAsync_Commited_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();
            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            try
            {
                var customerCountBefore = await Database.Query<Customers>()
                    .CountAsync();

                var result = await Database.WithTransactionAsync(async transaction =>
                {
                    await customer.InsertAsync(dbTransaction: transaction);
                });

                var customerCountAfter = await Database.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore + 1, customerCountAfter, "Customer wasnt inserted");
                Assert.IsTrue(result, "Transaction wasnt successful");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task WithTransactionAsync_Rollback_MySql()
        {
            TestDatabaseFixture.UseMySqlConnection();
            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            try
            {
                var customerCountBefore = await Database.Query<Customers>()
                    .CountAsync();

                var result = await Database.WithTransactionAsync(async transaction =>
                {
                    await customer.InsertAsync(dbTransaction: transaction);
                    throw new Exception("Force rollback");
                });

                var customerCountAfter = await Database.Query<Customers>()
                    .CountAsync();

                Assert.AreEqual(customerCountBefore, customerCountAfter, "Customer was inserted");
                Assert.IsFalse(result, "Transaction was successful");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }
    }
}
