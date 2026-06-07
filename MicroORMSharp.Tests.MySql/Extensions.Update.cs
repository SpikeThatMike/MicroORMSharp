using MicroORMSharp.SqlGenerator;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    public partial class Extensions
    {
        [TestMethod]
        [DoNotParallelize]
        public void Update()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customer);

            try
            {
                customer = customer.Insert();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                var updated = customer.Update();

                Assert.AreEqual("Mike", updated.Forename, "Selected column did not update");
                Assert.AreNotEqual(originalSurname, updated.Surname, "Surname didnt update");
                Assert.AreNotEqual(originalAddressLine1, updated.AddressLine1, "AddressLine1 didnt update");

                var fromDatabase = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingle();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreNotEqual(originalSurname, fromDatabase.Surname, "Surname hasnt updated");
                Assert.AreNotEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 hasnt updated");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                customer = await customer.InsertAsync();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                var updated = await customer.UpdateAsync();

                Assert.AreEqual("Mike", updated.Forename, "Selected column did not update");
                Assert.AreNotEqual(originalSurname, updated.Surname, "Surname didnt update");
                Assert.AreNotEqual(originalAddressLine1, updated.AddressLine1, "AddressLine1 didnt update");

                var fromDatabase = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingleAsync();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreNotEqual(originalSurname, fromDatabase.Surname, "Surname hasnt updated");
                Assert.AreNotEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 hasnt updated");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void UpdateOnly()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customer);

            try
            {
                customer = customer.Insert();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                customer.UpdateOnly();

                var fromDatabase = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingle();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreNotEqual(originalSurname, fromDatabase.Surname, "Surname hasnt updated");
                Assert.AreNotEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 hasnt updated");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateOnlyAsync()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

            try
            {
                customer = await customer.InsertAsync();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                await customer.UpdateOnlyAsync();

                var fromDatabase = await Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingleAsync();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreNotEqual(originalSurname, fromDatabase.Surname, "Surname hasnt updated");
                Assert.AreNotEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 hasnt updated");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }


        [TestMethod]
        [DoNotParallelize]
        public void Update_SelectColumns()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customer);

            try
            {
                customer = customer.Insert();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                var updated = customer.Update(x => new { x.Forename });

                Assert.AreEqual("Mike", updated.Forename, "Selected column did not update");
                Assert.AreEqual(originalSurname, updated.Surname, "Unselected columns updated");
                Assert.AreEqual(originalAddressLine1, updated.AddressLine1, "Unselected columns updated");

                var fromDatabase = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingle();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreEqual(originalSurname, fromDatabase.Surname, "Surname has updated");
                Assert.AreEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 has updated");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateAsync_SelectColumns()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

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
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void UpdateOnly_SelectColumns()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            TestDatabaseFixture.EnsureTableCreated(customer);

            try
            {
                customer = customer.Insert();

                var originalSurname = customer.Surname;
                var originalAddressLine1 = customer.AddressLine1;

                customer.Forename = "Mike";
                customer.Surname = "Test value which will not be updated";
                customer.AddressLine1 = "Somewhere made up which will not be updated";

                customer.UpdateOnly(x => new { x.Forename });

                var fromDatabase = Database.Query<Customers>()
                    .Where(x => x.Id == customer.Id)
                    .ExecuteSingle();

                Assert.AreEqual("Mike", fromDatabase.Forename, "Forename has not updated");
                Assert.AreEqual(originalSurname, fromDatabase.Surname, "Surname has updated");
                Assert.AreEqual(originalAddressLine1, fromDatabase.AddressLine1, "AddressLine1 has updated");
            }
            finally
            {
                TestDatabaseFixture.AssertTableDropped(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task UpdateOnlyAsync_SelectColumns()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = TestDatabaseFixture.CreateCustomer();
            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);

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
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }
    }
}
