using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.Models
{
    public class Car
    {
        public int Id { get; set; }

        public string Vendor { get; set; }

        public string Model { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ApplicationUser Owner { get; set; }

        public List<Record> Records {get;set;}
    }
}
