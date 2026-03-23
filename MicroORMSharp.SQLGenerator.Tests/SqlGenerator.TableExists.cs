namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void TableExists_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);

            var sqlQuery = sqlGenerator.TableExists();
            AssertQuery(
                sqlQuery,
                "SELECT 1 FROM information_schema.TABLES WHERE TABLE_NAME = @databaseTable",
                "TableExists query does not match"
            );

            Assert.AreEqual(1, sqlQuery.Parameters.Count, "Unexpected parameter count");
            Assert.AreEqual("Customers", sqlQuery.Parameters["databaseTable"], "Unexpected table parameter");
        }

        [TestMethod]
        public void TableExists_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);

            var sqlQuery = sqlGenerator.TableExists();
            AssertQuery(
                sqlQuery,
                "SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA = @databaseSchema AND TABLE_NAME = @databaseTable",
                "TableExists query does not match"
            );

            Assert.AreEqual(2, sqlQuery.Parameters.Count, "Unexpected parameter count");
            Assert.AreEqual("dbo", sqlQuery.Parameters["databaseSchema"], "Unexpected schema parameter");
            Assert.AreEqual("Customers", sqlQuery.Parameters["databaseTable"], "Unexpected table parameter");
        }
    }
}
