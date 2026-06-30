namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        [TestMethod]
        public void CreateTable_MySql()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.CreateTable();

            AssertQuery(
                sqlQuery,
                "CREATE TABLE `Customers` (Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY, Forename LONGTEXT, Surname LONGTEXT, AddressLine1 LONGTEXT, AddressLine2 LONGTEXT, AddressLine3 LONGTEXT, AddressLine4 LONGTEXT, Postalcode VARCHAR(10), Nullable INT, NotNullable INT NOT NULL, Active BIT NOT NULL)",
                "Create table queries do not match"
            );
        }

        [TestMethod]
        public void CreateTable_SqlServer()
        {
            var sqlGenerator = CreateCustomerGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.CreateTable();

            AssertQuery(
                sqlQuery,
                "CREATE TABLE [dbo].[Customers] (Id BIGINT NOT NULL IDENTITY(1,1), Forename VARCHAR(MAX), Surname VARCHAR(MAX), AddressLine1 VARCHAR(MAX), AddressLine2 VARCHAR(MAX), AddressLine3 VARCHAR(MAX), AddressLine4 VARCHAR(MAX), Postalcode VARCHAR(10), Nullable INT, NotNullable INT NOT NULL, Active BIT NOT NULL)",
                "Create table queries do not match"
            );
        }
    }
}
