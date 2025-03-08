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
                "DROP TABLE `Customers`",
                query,
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
                "DROP TABLE [dbo].[Customers]",
                query,
                "Drop table queries do not match"
            );
        }
    }
}
