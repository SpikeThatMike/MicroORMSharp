using MicroORMSharp.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroORMSharp.Helpers;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public sealed class DapperWrapperTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            //Add connection to MySql DB - Test local db
            Database.AddConnectionString(
                DatabaseType.MySql,
                "MySql",
                "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;AllowLoadLocalInfile=true",
                allowTableExtensions: true
            );
        }

        [TestMethod]
        public async Task QueryAsync_MySql()
        {
            var customer = new Customers
            {
                Forename = "John",
                Surname = "Doe",
                AddressLine1 = "123 Fake Street",
                AddressLine2 = "Fakeville",
                AddressLine3 = "Faketon",
                AddressLine4 = "Fakeshire",
                Postcode = "FA1 2KE",
                Nullable = 1,
                NotNullable = 2
            };

            var exists = await customer.TableExistsAsync();
            if (!exists)
            {
                await customer.CreateTableAsync();
                var isCreated = await customer.TableExistsAsync();
                Assert.IsTrue(isCreated, "Failed to create table");
            }

            customer = await customer.InsertAsync();
            Assert.IsTrue(customer.Id > 0, "Failed to retrieve data from insert");

            var results = await Database.Dapper.QueryAsync<string>(
                $"SELECT `Forename` FROM {Helper.GetTableName<Customers>()};"
            );

            Assert.AreEqual(1, results.Count(), "Incorrect result count");

            await customer.DropTableAsync();
            var isDeleted = await customer.TableExistsAsync();

            Assert.IsFalse(isDeleted, "Failed to delete table");
        }
    }
}
