using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("AttributeTestTable")]
    public class AttributeTestTable : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }

        [DbMaxLength(20)]
        [DbDefault("guest")]
        public string? Name { get; set; }

        [DbPrecision(10, 3)]
        [DbDefault(12.345)]
        public decimal? Amount { get; set; }

        [DbDefault(7)]
        public int? Quantity { get; set; }

        [DbDefault(true)]
        public bool? IsEnabled { get; set; }

        public byte[]? Payload { get; set; }
    }
}
