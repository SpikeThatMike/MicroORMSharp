namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void TruncateTable_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.TruncateTable();

            AssertQuery(sqlQuery, "TRUNCATE TABLE `Customers`", "Truncate table queries do not match");
        }

        [TestMethod]
        public void TruncateTable_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.TruncateTable();

            AssertQuery(sqlQuery, "TRUNCATE TABLE [dbo].[Customers]", "Truncate table queries do not match");
        }
    }
}
