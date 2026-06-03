using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.Tests.Models
{
    [DbTable("NestedJoinOrderStatus")]
    public class NestedJoinOrderStatus : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
