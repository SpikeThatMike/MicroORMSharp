using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.Tests.Models
{
    [DbTable("LeftJoinOrder")]
    public class LeftJoinOrder : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long CustomerId { get; set; }
    }
}
