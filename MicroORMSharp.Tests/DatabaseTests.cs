using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public sealed class DatabaseTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            //Add connection to MySql DB - Test local db
            Database.AddConnectionString(
                DatabaseType.MySql,
                "MySql",
                "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;AllowLoadLocalInfile=true",
                allowTableExtensions: true
            );
        }

        [TestMethod]
        public async Task Execute_MySql()
        {

        }

        [TestMethod]
        public async Task ExecuteSingle_MySql()
        {

        }

        [TestMethod]
        public async Task Any_MySql()
        {

        }

        [TestMethod]
        public async Task Count_MySql()
        {

        }
    }
}
