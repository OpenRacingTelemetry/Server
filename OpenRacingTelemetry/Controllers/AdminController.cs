using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using OpenRacingTelemetry.Data;
using OpenRacingTelemetry.Models;
using OpenRacingTelemetry.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRacingTelemetry.Controllers
{
    // [Authorize(Roles="Administrator")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        private ApplicationDbContext context;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Application Roles
        [HttpGet]
        public IActionResult ApplicationRoles()
        {
            List<ApplicationRoleListViewModel> model = new List<ApplicationRoleListViewModel>();

            model = roleManager.Roles.Select(r => new ApplicationRoleListViewModel()
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description,
                NumberOfUsers = r.Users.Count
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationRolesAddEdit(string id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    model.Id = applicationRole.Id;
                    model.RoleName = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
            }
            return PartialView("_ApplicationRolesAddEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> ApplicationRolesAddEdit(string id, ApplicationRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isExist = !String.IsNullOrEmpty(id);
                ApplicationRole applicationRole = isExist ? await roleManager.FindByIdAsync(id) :
               new ApplicationRole
               {
                   CreatedDate = DateTime.UtcNow
               };
                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                IdentityResult roleRuslt = isExist ? await roleManager.UpdateAsync(applicationRole)
                                                    : await roleManager.CreateAsync(applicationRole);
                if (roleRuslt.Succeeded)
                {
                    return RedirectToAction("ApplicationRoles");
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ApplicationRolesDelete(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    name = applicationRole.Name;
                }
            }
            return PartialView("_ApplicationRolesDelete", name);
        }


        [HttpPost]
        public async Task<IActionResult> ApplicationRolesDelete(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await roleManager.FindByIdAsync(id);
                if (applicationRole != null)
                {
                    IdentityResult roleRuslt = roleManager.DeleteAsync(applicationRole).Result;
                    if (roleRuslt.Succeeded)
                    {
                        return RedirectToAction("ApplicationRoles");
                    }
                }
            }
            return View();
        }
        #endregion

        #region Users
        [HttpGet]
        public IActionResult Users()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            model = userManager.Users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            return View(model);
        }


        [HttpGet]
        public IActionResult UsersAdd()
        {
            UserViewModel model = new UserViewModel()
            {
                ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList()
            };
            return PartialView("_UsersAdd", model);
        }


        [HttpPost]
        public async Task<IActionResult> UsersAdd(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                    if (applicationRole != null)
                    {
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                        if (roleResult.Succeeded)
                        {
                            return RedirectToAction("Users");
                        }
                    }
                }
            }
            return PartialView("_UsersAdd", model);
        }

        [HttpGet]
        public async Task<IActionResult> UsersEdit(string id)
        {
            EditUserViewModel model = new EditUserViewModel()
            {
                ApplicationRoles = roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList()
            };
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;

                    model.ApplicationRoleId = roleManager.Roles.SingleOrDefault(r => r.Name == userManager.GetRolesAsync(user).Result.SingleOrDefault())?.Id;
                }
            }
            return PartialView("_UsersEdit", model);
        }


        [HttpPost]
        public async Task<IActionResult> UsersEdit(string id, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.Email;

                    string existingRole = userManager.GetRolesAsync(user).Result.SingleOrDefault();
                    string existingRoleId = roleManager.Roles.SingleOrDefault(r => r.Name == existingRole)?.Id;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (existingRoleId != model.ApplicationRoleId)
                        {
                            IdentityResult roleResult = null;

                            if (existingRole != null)
                            {
                                roleResult = await userManager.RemoveFromRoleAsync(user, existingRole);
                            }

                            if (roleResult == null || roleResult.Succeeded)
                            {
                                ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRoleId);
                                if (applicationRole != null)
                                {
                                    IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);
                                    if (newRoleResult.Succeeded)
                                    {
                                        return RedirectToAction("Users");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return PartialView("_UsersEdit", model);
        }


        [HttpGet]
        public async Task<IActionResult> UsersDelete(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    name = $"{applicationUser.FirstName} {applicationUser.LastName}";
                }
            }
            return PartialView("_UsersDelete", name);
        }


        [HttpPost]
        public async Task<IActionResult> UsersDelete(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await userManager.DeleteAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                }
            }
            return View();
        }
        #endregion 

        #region OrganizersTeam
        [HttpGet]
        public IActionResult OrganizersTeam()
        {
            List<OrganizersTeamListViewModel> model = new List<OrganizersTeamListViewModel>();
            model = context.OrganizersTeams.Select(u => new OrganizersTeamListViewModel
            {
                ID = u.Id,
                Name = u.Name,
                Description = u.Description
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult OrganizersTeamAdd()
        {
            return PartialView("_OrganizersTeamAdd");
        }


        [HttpPost]
        public async Task<IActionResult> OrganizersTeamAdd(OrganizersTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                OrganizersTeam user = new OrganizersTeam
                {
                    Name = model.Name,
                    Description = model.Description
                };

                context.OrganizersTeams.Add(user);
                await context.SaveChangesAsync();
                return RedirectToAction("OrganizersTeam");

            }
            return PartialView("_OrganizersTeamAdd", model);
        }

        [HttpGet]
        public IActionResult OrganizersTeamEdit(string id)
        {
            OrganizersTeamViewModel model = new OrganizersTeamViewModel();

            if (!String.IsNullOrEmpty(id))
            {
                var team = context.OrganizersTeams.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
                if (team != null)
                {
                    model.Name = team.Name;
                    model.Description = team.Description;
                }
            }
            return PartialView("_OrganizersTeamEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> OrganizersTeamEdit(string id, OrganizersTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var team = context.OrganizersTeams.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
                if (team != null)
                {
                    team.Name = model.Name;
                    team.Description = model.Description;

                    await context.SaveChangesAsync();
                 
                    return RedirectToAction("OrganizersTeam");

                }
            }
            return PartialView("_OrganizersTeamEdit", model);
        }


        [HttpGet]
        public IActionResult OrganizersTeamDelete(string id)
        {
            string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                var team = context.OrganizersTeams.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
                if (team != null)
                {
                    name = $"{team.Name}";
                }
            }
            return PartialView("_OrganizersTeamDelete", name);
        }

        [HttpPost]
        public async Task<IActionResult> OrganizersTeamDelete(string id, IFormCollection form)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var team = context.OrganizersTeams.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
                if (team != null)
                {
                    context.OrganizersTeams.Remove(team);
                    await context.SaveChangesAsync();
                    return RedirectToAction("OrganizersTeam");
                }
            }
            return View();
        }


        #endregion 
    }
}
