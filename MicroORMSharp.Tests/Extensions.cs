using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.Tests
{
    [TestClass]
    public partial class Extensions
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            //Add connection to MySql DB - Test local db
            Database.AddConnectionString(DatabaseType.MySql, "MySql", "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;AllowLoadLocalInfile=true");
        }
    }
}
