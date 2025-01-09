using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void DropTable_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            var createTable = sqlGenerator.DropTable();
            var query = createTable.ToString();

            Assert.AreEqual(
                query,
                "DROP TABLE `Customers`",
                "Drop table queries do not match"
            );
        }

        [TestMethod]
        public void DropTable_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            var createTable = sqlGenerator.DropTable();
            var query = createTable.ToString();

            Assert.AreEqual(
                query,
                "DROP TABLE [dbo].[Customers]",
                "Drop table queries do not match"
            );
        }
    }
}
