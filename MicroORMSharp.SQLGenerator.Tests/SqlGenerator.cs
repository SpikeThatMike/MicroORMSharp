using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    [TestClass]
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void Setup()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            Assert.AreEqual("[dbo].[Customers]", sqlGenerator.GetFullTableName());
            Assert.AreEqual(1, sqlGenerator.IgnoreProperties.Count());
            Assert.AreEqual("FullName", sqlGenerator.IgnoreProperties.First().Name);
        }
    }
}
