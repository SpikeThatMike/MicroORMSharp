using MicroORMSharp.SqlGenerator;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateAsync_SelectColumns_MySql()
        {
            UseMySqlConnection();

            var customer = CreateCustomer();
            await EnsureTableCreatedAsync(customer);

            try
            {
                customer = await customer.InsertAsync();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                var updated = await customer.UpdateAsync(x => new { x.Forename });

                Assert.AreEqual("Mike", updated.Forename, "Selected column did not update");
                Assert.AreEqual(originalSurname, updated.Surname, "Unselected columns updated");
                Assert.AreEqual(originalAddressLine1, updated.AddressLine1, "Unselected columns updated");

                var fromDatabase = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingleAsync();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreEqual(originalSurname, fromDatabase.Surname, "Surname has updated");
                Assert.AreEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 has updated");
            }
            finally
            {
                await AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateOnlyAsync_SelectColumns_MySql()
        {
            UseMySqlConnection();

            var customer = CreateCustomer();
            await EnsureTableCreatedAsync(customer);

            try
            {
                customer = await customer.InsertAsync();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                await customer.UpdateOnlyAsync(x => new { x.Forename });

                var fromDatabase = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingleAsync();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreEqual(originalSurname, fromDatabase.Surname, "Surname has updated");
                Assert.AreEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 has updated");
            }
            finally
            {
                await AssertTableDroppedAsync(customer);
            }
        }
    }
}
