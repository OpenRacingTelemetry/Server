using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.Models
{
    public class Place
    {
        public int PlaceId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<Race> Races { get; set; }

    }
}
