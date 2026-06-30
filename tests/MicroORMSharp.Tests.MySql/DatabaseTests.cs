using MicroORMSharp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public class DatabaseTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Database.Initialise();
            TestDatabaseFixture.EnsureMySqlConnection();
        }
    }
}
