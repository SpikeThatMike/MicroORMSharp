using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System.Collections.Generic;

namespace MicroORMSharp.Tests.MySql.Models
{
    [DbTable("LeftJoinCustomer")]
    public class LeftJoinCustomer : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [DBJoin(typeof(LeftJoinOrder), "Id", "CustomerId", DBJoinType.Left)]
        public List<LeftJoinOrder> Orders { get; set; } = new List<LeftJoinOrder>();
    }
}
