using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroORMSharp.SqlGenerator;

namespace MicroORMSharp.Tests
{
    internal static class TestDatabaseFixture
    {
        private const string MySqlReference = "MySql";
        private const string MySqlConnectionString = "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;AllowLoadLocalInfile=true";

        public static void EnsureMySqlConnection()
        {
            if (Database.GetAllConnections().All(x => x.Reference != MySqlReference))
            {
                Database.AddConnectionString(
                    DatabaseType.MySql,
                    MySqlReference,
                    MySqlConnectionString,
                    allowTableExtensions: true
                );
            }
        }

        public static void UseMySqlConnection()
        {
            Database.SetConnectionString(MySqlReference);
        }

        public static Customers CreateCustomer(string suffix = "")
        {
            return new Customers
            {
                Forename = $"John{suffix}",
                Surname = $"Doe{suffix}",
                AddressLine1 = "Test Street",
                AddressLine2 = "Test Town",
                AddressLine3 = "Test City",
                AddressLine4 = "Test County",
                Postcode = "Postcode",
                Nullable = null,
                NotNullable = 0,
                Active = true,
            };
        }

        public static List<Customers> CreateCustomerBatch()
        {
            return new List<Customers>
            {
                CreateCustomer(" 1"),
                CreateCustomer(" 2")
            };
        }

        public static async Task EnsureTableCreatedAsync(Customers customers)
        {
            var exists = await customers.TableExistsAsync();
            if (!exists)
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
        }

        public static async Task EnsureTableCreatedAsync(List<Customers> customers)
        {
            var exists = await customers.TableExistsAsync();
            if (!exists)
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
        }

        public static async Task DropTableIfExistsAsync(Customers customers)
        {
            if (await customers.TableExistsAsync())
                await customers.DropTableAsync();
        }

        public static async Task AssertTableDroppedAsync(Customers customers)
        {
            await customers.DropTableAsync();
            var isDeleted = await customers.TableExistsAsync();
            Assert.IsFalse(isDeleted, "Failed to delete table");
        }

        public static async Task AssertTableDroppedAsync(List<Customers> customers)
        {
            await customers.DropTableAsync();
            var isDeleted = await customers.TableExistsAsync();
            Assert.IsFalse(isDeleted, "Failed to delete table");
        }

    }
}
