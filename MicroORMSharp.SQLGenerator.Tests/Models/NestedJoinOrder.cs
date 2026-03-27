using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("NestedOrder")]
    public class NestedJoinOrder : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public long StatusId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        [DBJoin(typeof(NestedJoinOrderStatus), "StatusId", "Id", DBJoinType.Inner)]
        public NestedJoinOrderStatus OrderStatus { get; set; }
    }
}
