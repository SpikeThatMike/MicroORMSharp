namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void DropTable_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.DropTable();

            AssertQuery(sqlQuery, "DROP TABLE `Customers`", "Drop table queries do not match");
        }

        [TestMethod]
        public void DropTable_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.DropTable();

            AssertQuery(sqlQuery, "DROP TABLE [dbo].[Customers]", "Drop table queries do not match");
        }
    }
}
