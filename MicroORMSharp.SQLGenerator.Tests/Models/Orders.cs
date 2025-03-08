using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("Orders")]
    public class Orders : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public decimal Price { get; set; }
    }
}
