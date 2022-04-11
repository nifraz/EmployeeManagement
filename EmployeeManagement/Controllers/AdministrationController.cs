using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Authorize(Roles = "Admin,User")]   //user must be either one
    //[Authorize(Roles = "Admin")][Authorize(Roles = "User")] //user must be all
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult ListRoles()
        {
            return View(roleManager.Roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole()
                {
                    Name = model.RoleName
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == default)
            {
                ViewBag.ErrorMsg = $"The role id {id} cannot be found.";
                return View("Error");
            }

            var model = new RoleEditViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var users = await userManager.GetUsersInRoleAsync(role.Name);
            //foreach (var user in await userManager.Users.ToListAsync())
            //{                    
            //    if (await userManager.IsInRoleAsync(user, role.Name))
            //    {
            //        model.Users.Add(user.UserName);
            //    }         
            //}
            foreach (var user in users)
            {
                model.Users.Add(user.UserName);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == default)
            {
                ViewBag.ErrorMsg = $"The role id {model.Id} cannot be found.";
                return View("Error");
            }

            role.Name = model.RoleName;

            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == default)
            {
                ViewBag.ErrorMsg = $"The role id {roleId} cannot be found.";
                return View("Error");
            }

            var model = new List<RoleUserViewModel>();

            foreach (var user in userManager.Users.ToList())
            {
                var roleUser = new RoleUserViewModel()
                {
                    UserId = user.Id,
                    Username = user.UserName
                };
                roleUser.IsSelected = await userManager.IsInRoleAsync(user, role.Name);
                model.Add(roleUser);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(IList<RoleUserViewModel> model, string roleId)
        {
            ViewBag.RoleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == default)
            {
                ViewBag.ErrorMsg = $"The role id {roleId} cannot be found.";
                return View("Error");
            }

            foreach (var roleUser in model)
            {
                var user = await userManager.FindByIdAsync(roleUser.UserId);
                if (user == default)
                {
                    continue;
                }

                //var resultList = new List<IdentityResult>();

                if (roleUser.IsSelected)
                {
                    if (!await userManager.IsInRoleAsync(user, role.Name))
                    {
                        await userManager.AddToRoleAsync(user, role.Name);  //add result to resultList
                        //foreach (var error in result.Errors)
                        //{
                        //    ModelState.AddModelError(string.Empty, $"User '{user.UserName}' : {error.Description}");
                        //    model[i].IsSelected = await this.userManager.IsInRoleAsync(user, role.Name);
                        //}
                    }
                }
                else
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }
            }

            return RedirectToAction("EditRole", "Administration", new { Id = role.Id });
        }
    }
}
