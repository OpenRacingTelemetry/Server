using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.Models
{
    public class Race
    {
        public int RaceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Weather { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime Date { get; set; }

        public int? PlaceId { get; set; }

        public Place Place { get; set; }

        public List<Record> Records {get;set;}

        public Race()
        {
            Records = new List<Record>();
        }
    }
}
