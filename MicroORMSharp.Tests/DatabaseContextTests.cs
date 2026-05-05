using MicroORMSharp.SqlGenerator;
using MicroORMSharp.Tests.Models;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public class DatabaseContextTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
        }

        [TestMethod]
        public void CreateContext_AssignsNamedConnectionMetadata()
        {
            using var context = Database.CreateContext(TestDatabaseFixture.MySqlReference);
            var query = context.Query<Customers>();

            Assert.AreEqual(DatabaseType.MySql, context.DatabaseType, "Database type doesnt match");
            Assert.AreEqual(DatabaseType.MySql, query._databaseType, "Database type doesnt match");
            Assert.AreSame(context._connection, query._dbConnection, "Connection doesnt match");
        }
    }
}
