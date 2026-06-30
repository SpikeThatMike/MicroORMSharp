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
        public void CreateAndDeleteTable(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = new Customers();

                TestDatabaseFixture.EnsureTableDropped(customers);

            try
            {
                customers.CreateTable();
                var isCreated = customers.TableExists();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
            finally
            {
                TestDatabaseFixture.EnsureTableDropped(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task CreateAndDeleteTableAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = new Customers();

            await TestDatabaseFixture.EnsureTableDroppedAsync(customers);

            try
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
            finally
            {
                await TestDatabaseFixture.EnsureTableDroppedAsync(customers);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public void TruncateTable(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customers);

            try
            {
                customers = customers.Insert();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

                customers.TruncateTable();
                var anyCustomers = Database.Query<Customers>().Any();

                Assert.IsFalse(anyCustomers, "Failed to truncate data from table");
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
        public async Task TruncateTableAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var customers = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customers);

            try
            {
                customers = await customers.InsertAsync();
                Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

                await customers.TruncateTableAsync();
                var anyCustomers = await Database.Query<Customers>().AnyAsync();

                Assert.IsFalse(anyCustomers, "Failed to truncate data from table");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customers);
            }
        }
    }
}
