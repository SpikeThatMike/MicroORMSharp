using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task BulkInsert_MySql()
        {
            Database.SetConnectionString("MySql");

            var customers = new List<Customers>()
            {
                new Customers()
                {
                    Forename = "John 1",
                    Surname = "Doe 1",
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Town",
                    AddressLine3 = "Test City",
                    AddressLine4 = "Test County",
                    Postcode = "Postcode",
                    Nullable = null,
                    NotNullable = 0,
                    Active = true,
                },
                new Customers()
                {
                    Forename = "John 2",
                    Surname = "Doe 2",
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Town",
                    AddressLine3 = "Test City",
                    AddressLine4 = "Test County",
                    Postcode = "Postcode",
                    Nullable = null,
                    NotNullable = 0,
                    Active = true,
                }
            };

            //initial check in case the table already exists
            var exists = await customers.TableExistsAsync();
            if (!exists)
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }

            customers.Insert();

            var data = await Database.Query<Customers>().ExecuteAsync();
            Assert.IsTrue(data.Count() == 2, "Failed to bulk insert data");

            await customers.DropTableAsync();
            var isDeleted = await customers.TableExistsAsync();

            Assert.IsFalse(isDeleted, "Failed to delete table");
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task BulkInsertAsync_MySql()
        {
            Database.SetConnectionString("MySql");

            var customers = new List<Customers>()
            {
                new Customers()
                {
                    Forename = "John 1",
                    Surname = "Doe 1",
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Town",
                    AddressLine3 = "Test City",
                    AddressLine4 = "Test County",
                    Postcode = "Postcode",
                    Nullable = null,
                    NotNullable = 0,
                    Active = true,
                },
                new Customers()
                {
                    Forename = "John 2",
                    Surname = "Doe 2",
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Town",
                    AddressLine3 = "Test City",
                    AddressLine4 = "Test County",
                    Postcode = "Postcode",
                    Nullable = null,
                    NotNullable = 0,
                    Active = true,
                }
            };

            //initial check in case the table already exists
            var exists = await customers.TableExistsAsync();
            if (!exists)
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }

            await customers.InsertAsync();

            var data = await Database.Query<Customers>().ExecuteAsync();
            Assert.IsTrue(data.Count() == 2, "Failed to bulk insert data");

            await customers.DropTableAsync();
            var isDeleted = await customers.TableExistsAsync();

            Assert.IsFalse(isDeleted, "Failed to delete table");
        }
    }
}
