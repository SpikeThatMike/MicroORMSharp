using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;

namespace MicroORMSharp.Tests.MySql.Models
{
    [DbTable("JoinTestOrder")]
    public class TestJoinOrder : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
