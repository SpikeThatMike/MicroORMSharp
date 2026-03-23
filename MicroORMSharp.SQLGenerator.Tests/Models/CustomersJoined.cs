using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("Customer")]
    public class CustomersJoined : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        [DBJoin(typeof(Order), "Id", "CustomerId")]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}

