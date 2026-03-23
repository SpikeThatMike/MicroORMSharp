using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.SqlGenerator.Tests.Models
{
    [DbTable("Customers")]
    public class Customers : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public string Forename { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddressLine3 { get; set; } = string.Empty;
        public string AddressLine4 { get; set; } = string.Empty;
        [DbColumn("Postalcode")]
        [DbMaxLength(10)]
        public string Postcode { get; set; } = string.Empty;

        public int? Nullable { get; set; }
        public int NotNullable { get; set; }
        public bool Active { get; set; }

        [DbIgnore]
        public string FullName
        {
            get
            {
                return Forename + " " + Surname;
            }
        }
    }
}

