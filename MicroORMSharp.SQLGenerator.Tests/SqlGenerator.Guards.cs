using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void InsertRow_Throws_When_NoIdentityColumn()
        {
            var sqlGenerator = new SqlGenerator<NoIdentityEntity>(DatabaseType.SqlServer);

            var ex = Assert.ThrowsException<Exception>(() => sqlGenerator.InsertRow(new NoIdentityEntity { Name = "Test" }));

            StringAssert.Contains(ex.Message, "No identity column found", "Unexpected exception message");
        }

        [TestMethod]
        public void UpdateRow_Throws_When_NoIdentityColumn()
        {
            var sqlGenerator = new SqlGenerator<NoIdentityEntity>(DatabaseType.SqlServer);

            var ex = Assert.ThrowsException<InvalidOperationException>(() => sqlGenerator.UpdateRow(new NoIdentityEntity { Name = "Test" }));

            Assert.AreEqual("No identity column found.", ex.Message, "Unexpected exception message");
        }

        [TestMethod]
        public void DeleteRow_Throws_When_NoIdentityColumn()
        {
            var sqlGenerator = new SqlGenerator<NoIdentityEntity>(DatabaseType.SqlServer);

            var ex = Assert.ThrowsException<InvalidOperationException>(() => sqlGenerator.DeleteRow(new NoIdentityEntity { Name = "Test" }));

            Assert.AreEqual("No identity column found.", ex.Message, "Unexpected exception message");
        }

        [DbTable("NoIdentityEntity")]
        private class NoIdentityEntity : IMicroORMSharp
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
