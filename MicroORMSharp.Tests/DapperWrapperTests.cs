using MicroORMSharp.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public sealed class DapperWrapperTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestDatabaseFixture.EnsureMySqlConnection();
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task QueryAsync_MySql()
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
    }
}
