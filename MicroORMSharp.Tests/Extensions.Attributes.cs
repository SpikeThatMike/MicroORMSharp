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
        public async Task DbPrecision_CreateInsertUpdate(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);

            var entity = new AttributeTestTable();

            try
            {
                if (await entity.TableExistsAsync())
                {
                    await entity.DropTableAsync();
                }

                await entity.CreateTableAsync();
                Assert.IsTrue(await entity.TableExistsAsync(), "Failed to create configured entity table");

                var inserted = await entity.InsertAsync();

                Assert.IsTrue(inserted.Id > 0, "Failed to retrieve inserted configured entity");
                Assert.AreEqual("guest", inserted.Name, "String default was not applied during insert");
                Assert.AreEqual(12.345m, inserted.Amount, "Decimal default was not applied during insert");
                Assert.AreEqual(7, inserted.Quantity, "Integer default was not applied during insert");
                Assert.AreEqual(true, inserted.IsEnabled, "Boolean default was not applied during insert");
                Assert.IsNull(inserted.Payload, "Unexpected payload value after insert");

                inserted.Name = null;
                inserted.Amount = null;
                inserted.Quantity = null;
                inserted.IsEnabled = null;
                inserted.Payload = [1, 2, 3];

                var updated = await inserted.UpdateAsync();

                Assert.AreEqual("guest", updated.Name, "String default was not applied during update");
                Assert.AreEqual(12.345m, updated.Amount, "Decimal default was not applied during update");
                Assert.AreEqual(7, updated.Quantity, "Integer default was not applied during update");
                Assert.AreEqual(true, updated.IsEnabled, "Boolean default was not applied during update");
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, updated.Payload, "Payload was not updated");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(entity);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task DbMaxLengthExceeded_InsertAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);
            var entity = new AttributeTestTable
            {
                Name = "abcdefghijklmnopqrstuvwxyz"
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await entity.InsertAsync());

            StringAssert.Contains(ex.Message, "DbMaxLength(20)");
            StringAssert.Contains(ex.Message, "Actual length: 26");
        }

        [TestMethod]
        [DoNotParallelize]
        [DataRow(DatabaseType.MySql)]
        [DataRow(DatabaseType.SqlServer)]
        public async Task DbMaxLengthExceeded_UpdateAsync(DatabaseType databaseType)
        {
            TestDatabaseFixture.UseConnection(databaseType);
            var entity = new AttributeTestTable
            {
                Id = 1,
                Name = "abcdefghijklmnopqrstuvwxyz"
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await entity.UpdateAsync());

            StringAssert.Contains(ex.Message, "DbMaxLength(20)");
            StringAssert.Contains(ex.Message, "Actual length: 26");
        }
    }
}
