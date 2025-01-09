using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORMSharp.ExampleModels
{
    [DbTable("Customers")]
    public class Customers : IMicroORMSharp
    {
        [DbIdentity]
        public long Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        [DbColumn("Postalcode")]
        [DbMaxLength(10)]
        public string Postcode { get; set; }

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
