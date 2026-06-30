using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System.Collections.Generic;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("JoinTypeCustomer")]
    public class JoinTypeCustomer : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public string Name { get; set; }

        [DBJoin(typeof(JoinTypeOrder), "Id", "CustomerId", DBJoinType.Left)]
        public List<JoinTypeOrder> Orders { get; set; }
    }
}
