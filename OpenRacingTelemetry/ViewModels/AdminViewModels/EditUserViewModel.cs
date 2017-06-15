using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }


        [Display(Name = "First name")]
        public string FirstName { get; set; }


        [Display(Name = "Last name")]
        public string LastName { get; set; }


        public string Email { get; set; }


        public List<SelectListItem> ApplicationRoles { get; set; }


        [Display(Name = "Role")]
        public string ApplicationRoleId { get; set; }

    }
}
