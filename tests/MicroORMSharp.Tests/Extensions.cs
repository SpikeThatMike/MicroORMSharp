using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.Tests.Helpers;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public partial class Extensions
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
            TestDatabaseFixture.EnsureSqlServerConnection();
        }
    }
}
