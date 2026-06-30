using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("Order")]
    public class Order : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

