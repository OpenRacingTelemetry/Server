using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.Models
{
    public class OrganizersTeam
    {
        public int Id { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
