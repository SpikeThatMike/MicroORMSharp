namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void DeleteRow_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.DeleteRow(CreateCustomer());

            AssertQuery(sqlQuery, "DELETE FROM `Customers` WHERE Id = @p1", "Delete row queries do not match");
        }

        [TestMethod]
        public void DeleteRow_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.DeleteRow(CreateCustomer());

            AssertQuery(sqlQuery, "DELETE FROM [dbo].[Customers] WHERE Id = @p1", "Delete row queries do not match");
        }
    }
}
