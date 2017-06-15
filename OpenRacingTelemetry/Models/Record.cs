using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.Models
{
    public class Record
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int? CarId { get; set; }

        public Car Car { get; set; }

        public int RaceId { get; set; }

        public Race Race { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        public DateTime TimeStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        public DateTime TimeEnd { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        public DateTime TimePenalty { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss.fff}")]
        [NotMapped]
        public DateTime TimeTotal => TimePenalty.Add(TimeEnd.Subtract(TimeStart));


        public string Notes { get; set; }
    }
}
