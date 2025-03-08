using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void TruncateTable_MySql()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.MySql);

            var createTable = sqlGenerator.TruncateTable();
            var query = createTable.ToString();

            Assert.AreEqual(
                "TRUNCATE TABLE `Customers`",
                query,
                "Truncate table queries do not match"
            );
        }

        [TestMethod]
        public void TruncateTable_SqlServer()
        {
            var sqlGenerator = new SqlGenerator<Customers>(DatabaseType.SqlServer);

            var createTable = sqlGenerator.TruncateTable();
            var query = createTable.ToString();

            Assert.AreEqual(
                "TRUNCATE TABLE [dbo].[Customers]",
                query,
                "Truncate table queries do not match"
            );
        }
    }
}
