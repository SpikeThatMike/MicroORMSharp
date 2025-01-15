using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public partial class Extensions
    {
        public Extensions()
        {
            //Create 
            Database.AddConnectionString(DatabaseType.MySql, "SqlServer", "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;");
        }

        [TestMethod]
        public async Task CreateAndDeleteTable()
        {
            var customers = new Customers();

            //initial check in case the table already exists
            var alreadyExists = await customers.TableExistsAsync();
            if (alreadyExists)
            {
                await customers.DropTableAsync();
                var isDeleted = await customers.TableExistsAsync();
                Assert.IsFalse(isDeleted, "Failed to delete table");
            }

            await customers.CreateTableAsync();

            var isCreated = await customers.TableExistsAsync();

            Assert.IsTrue(isCreated, "Failed to create table");

            if (isCreated)
            {
                await customers.DropTableAsync();
                var isDeleted = await customers.TableExistsAsync();

                Assert.IsFalse(isDeleted, "Failed to delete table");
            }
        }

        [TestMethod]
        public async Task TruncateTable()
        {
            var customers = new Customers()
            {
                Forename = "John",
                Surname = "Doe",
                AddressLine1 = "Test Street",
                AddressLine2 = "Test Town",
                AddressLine3 = "Test City",
                AddressLine4 = "Test County",
                Postcode = "Postcode",
                Nullable = null,
                NotNullable = 0,
                Active = true,
            };

            //initial check in case the table already exists
            var alreadyExists = await customers.TableExistsAsync();
            if (alreadyExists)
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }

            customers = await customers.InsertAsync();

            Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

            await customers.TruncateTableAsync();

            var anyCustomers = await Database.Query<Customers>().AnyAsync();

            Assert.IsFalse(anyCustomers, "Failed to truncate data from table");

            await customers.DropTableAsync();
            var isDeleted = await customers.TableExistsAsync();

            Assert.IsFalse(isDeleted, "Failed to delete table");
        }
    }
}
