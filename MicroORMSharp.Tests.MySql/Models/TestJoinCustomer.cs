using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;

namespace MicroORMSharp.Tests.MySql.Models
{
    [DbTable("JoinTestCustomer")]
    public class TestJoinCustomer : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }

        [DBJoin(typeof(TestJoinOrder), "Id", "CustomerId", DBJoinType.Left)]
        public List<TestJoinOrder> Orders { get; set; }
    }
}
