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
        public async Task DeleteRow_MySql()
        {
            Database.SetConnectionString("MySql");

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
            var exists = await customers.TableExistsAsync();
            if (!exists)
            {
                await customers.CreateTableAsync();
                var isCreated = await customers.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }

            customers = await customers.InsertAsync();

            Assert.IsTrue(customers.Id > 0, "Failed to retrieve data from insert");

            await customers.DeleteAsync();
            
            var query = await Database.Query<Customers>().ExecuteAsync();

            Assert.IsTrue(query.Count() == 0, "Failed to delete row");
        }
    }
}
