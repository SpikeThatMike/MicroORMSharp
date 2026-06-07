using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public partial class Extensions
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
        }
    }
}
