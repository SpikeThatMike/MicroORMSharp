using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System.Collections.Generic;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("NestedCustomer")]
    public class NestedJoinCustomer : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [DBJoin(typeof(NestedJoinOrder), "Id", "CustomerId", DBJoinType.Left)]
        public List<NestedJoinOrder> Orders { get; set; } 
    }
}
