using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using MicroORMSharp.Tests.Models;
using MicroORMSharp.Tests.Helpers;

namespace MicroORMSharp.Tests.SqlServer
{
    [TestClass]
    public class DatabaseTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureSqlServerConnection();
        }
    }
}
