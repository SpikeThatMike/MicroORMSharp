using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("JoinTypeOrder")]
    public class JoinTypeOrder : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public long StatusId { get; set; }

        [DBJoin(typeof(JoinTypeStatus), "StatusId", "Id", DBJoinType.Inner)]
        public JoinTypeStatus Status { get; set; }
    }
}
