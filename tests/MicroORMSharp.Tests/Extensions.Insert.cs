using MicroORMSharp.SqlGenerator;
using MicroORMSharp.Tests.Helpers;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public void Insert(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task InsertAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                customers = await customers.InsertAsync();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task InsertOnly(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                var beforeCount = Database.Query<Customers>().Count();
                customers.InsertOnly();
                var afterCount = Database.Query<Customers>().Count();
                Assert.AreEqual(beforeCount + 1, afterCount, "Insert failed");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task InsertOnlyAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                var beforeCount = await Database.Query<Customers>().CountAsync();
                await customers.InsertOnlyAsync();
                var afterCount = await Database.Query<Customers>().CountAsync();
                Assert.AreEqual(beforeCount + 1, afterCount, "Insert failed");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
