using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.Tests.MySql
{
    internal static class TestDatabaseFixture
    {
        internal const string MySqlReference = "MySql";
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

        public static void EnsureTableCreated<T>(T table) where T : IMicroORMSharp
        {
            var exists = table.TableExists();
            if (!exists)
            {
                table.CreateTable();
                var isCreated = table.TableExists();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
        }

        public static void EnsureTableCreated<T>(List<T> table) where T : IMicroORMSharp
        {
            var exists = table.TableExists();
            if (!exists)
            {
                table.CreateTable();
                var isCreated = table.TableExists();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
        }

        public static void AssertTableDropped<T>(T table) where T : IMicroORMSharp
        {
            table.DropTable();
            var isDeleted = table.TableExists();
            Assert.IsFalse(isDeleted, "Failed to delete table");
        }

        public static void AssertTableDropped<T>(List<T> table) where T : IMicroORMSharp
        {
            table.DropTable();
            var isDeleted = table.TableExists();
            Assert.IsFalse(isDeleted, "Failed to delete table");
        }


        public static async Task EnsureTableCreatedAsync<T>(T table) where T : IMicroORMSharp
        {
            var exists = await table.TableExistsAsync();
            if (!exists)
            {
                await table.CreateTableAsync();
                var isCreated = await table.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
        }

        public static async Task EnsureTableCreatedAsync<T>(List<T> table) where T : IMicroORMSharp
        {
            var exists = await table.TableExistsAsync();
            if (!exists)
            {
                await table.CreateTableAsync();
                var isCreated = await table.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }
        }

        public static async Task AssertTableDroppedAsync<T>(T table) where T : IMicroORMSharp
        {
            await table.DropTableAsync();
            var isDeleted = await table.TableExistsAsync();
            Assert.IsFalse(isDeleted, "Failed to delete table");
        }

        public static async Task AssertTableDroppedAsync<T>(List<T> table) where T : IMicroORMSharp
        {
            await table.DropTableAsync();
            var isDeleted = await table.TableExistsAsync();
            Assert.IsFalse(isDeleted, "Failed to delete table");
        }
    }
}
