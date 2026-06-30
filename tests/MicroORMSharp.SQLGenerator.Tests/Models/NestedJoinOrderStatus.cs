using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("NestedOrderStatus")]
    public class NestedJoinOrderStatus : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
