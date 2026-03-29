using MicroORMSharp.SqlGenerator.Tests.Models;

namespace MicroORMSharp.SqlGenerator.Tests
{
    public sealed partial class SqlGenerator
    {
        private static SqlGenerator<AttributeTestTable> CreateAttributeGenerator(DatabaseType dbType)
        {
            return new SqlGenerator<AttributeTestTable>(dbType);
        }

        [TestMethod]
        public void CreateTable_Defaults_MySql()
        {
            var sqlGenerator = CreateAttributeGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.CreateTable();

            AssertQuery(
                sqlQuery,
                "CREATE TABLE `AttributeTestTable` (Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(20) DEFAULT 'guest', Amount DECIMAL(10, 3) DEFAULT 12.345, Quantity INT DEFAULT 7, IsEnabled BIT DEFAULT 1, Payload LONGBLOB)",
                "Configured entity create table query does not match"
            );
        }

        [TestMethod]
        public void CreateTable_Defaults_SqlServer()
        {
            var sqlGenerator = CreateAttributeGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.CreateTable();

            AssertQuery(
                sqlQuery,
                "CREATE TABLE [dbo].[AttributeTestTable] (Id BIGINT NOT NULL IDENTITY(1,1), Name VARCHAR(20) DEFAULT 'guest', Amount DECIMAL(10, 3) DEFAULT 12.345, Quantity INT DEFAULT 7, IsEnabled BIT DEFAULT 1, Payload VARBINARY(MAX))",
                "Configured entity create table query does not match"
            );
        }

        [TestMethod]
        public void InsertRow_UsesDefaultsForNullValues()
        {
            var sqlGenerator = CreateAttributeGenerator(DatabaseType.SqlServer);
            var sqlQuery = sqlGenerator.InsertRow(new AttributeTestTable());

            AssertQuery(
                sqlQuery,
                "INSERT INTO [dbo].[AttributeTestTable] (Name, Amount, Quantity, IsEnabled, Payload) VALUES (DEFAULT, DEFAULT, DEFAULT, DEFAULT, @p1); SELECT [AttributeTestTable].[Id] AS [Id], [AttributeTestTable].[Name] AS [Name], [AttributeTestTable].[Amount] AS [Amount], [AttributeTestTable].[Quantity] AS [Quantity], [AttributeTestTable].[IsEnabled] AS [IsEnabled], [AttributeTestTable].[Payload] AS [Payload] FROM [dbo].[AttributeTestTable] WHERE [AttributeTestTable].[Id] = (SELECT SCOPE_IDENTITY());",
                "Configured entity insert query does not match"
            );

            Assert.AreEqual(1, sqlQuery.Parameters.Count);
            Assert.IsTrue(sqlQuery.Parameters.ContainsKey("@p1"));
            Assert.IsNull(sqlQuery.Parameters["@p1"]);
        }

        [TestMethod]
        public void UpdateRow_UsesDefaultsForNullValues()
        {
            var sqlGenerator = CreateAttributeGenerator(DatabaseType.MySql);
            var sqlQuery = sqlGenerator.UpdateRow(new AttributeTestTable { Id = 10 }, false);

            AssertQuery(
                sqlQuery,
                "UPDATE `AttributeTestTable` SET `AttributeTestTable`.`Name` = DEFAULT, `AttributeTestTable`.`Amount` = DEFAULT, `AttributeTestTable`.`Quantity` = DEFAULT, `AttributeTestTable`.`IsEnabled` = DEFAULT, `AttributeTestTable`.`Payload` = @p2 WHERE `AttributeTestTable`.`Id` = @p1;",
                "Configured entity update query does not match"
            );

            Assert.AreEqual(2, sqlQuery.Parameters.Count);
            Assert.AreEqual(10L, sqlQuery.Parameters["@p1"]);
            Assert.IsNull(sqlQuery.Parameters["@p2"]);
        }

        [TestMethod]
        public void UpdateRow_MaxLength()
        {
            var sqlGenerator = CreateAttributeGenerator(DatabaseType.MySql);
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
                sqlGenerator.UpdateRow(new AttributeTestTable { Id = 1, Name = "abcdefghijklmnopqrstuvwxyz" }, false));

            StringAssert.Contains(ex.Message, "DbMaxLength(20)");
            StringAssert.Contains(ex.Message, "Actual length: 26");
        }

        [TestMethod]
        public void InsertRow_MaxLength()
        {
            var sqlGenerator = CreateAttributeGenerator(DatabaseType.MySql);
            var ex = Assert.ThrowsException<InvalidOperationException>(() =>
                sqlGenerator.InsertRow(new AttributeTestTable { Name = "abcdefghijklmnopqrstuvwxyz" }, false));

            StringAssert.Contains(ex.Message, "DbMaxLength(20)");
            StringAssert.Contains(ex.Message, "Actual length: 26");
        }
    }
}
