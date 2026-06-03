using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System.Collections.Generic;

namespace MicroORMSharp.Tests.MySql.Models
{
    [DbTable("NestedJoinCustomer")]
    public class NestedJoinCustomer : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public string Name { get; set; }

        [DBJoin(typeof(NestedJoinOrder), "Id", "CustomerId", DBJoinType.Left)]
        public List<NestedJoinOrder> Orders { get; set; }
    }
}
