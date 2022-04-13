using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            return View(userManager.Users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == default)
            {
                ViewBag.ErrorMsg = $"The user id {id} cannot be found.";
                return View("Error");
            }

            var roles = await userManager.GetRolesAsync(user);
            var claims = (await userManager.GetClaimsAsync(user)).Select(c => c.Value).ToList();

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                City = user.City,
                Roles = roles,
                Claims = claims
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);

                if (user == default)
                {
                    ViewBag.ErrorMsg = $"The user id {model.Id} cannot be found.";
                    return View("Error");
                }

                user.Email = model.Email;
                user.UserName = model.Username;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            ViewBag.UserId = userId;

            var user = await userManager.FindByIdAsync(userId);
            if (user == default)
            {
                ViewBag.ErrorMsg = $"The user id {userId} cannot be found.";
                return View("Error");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var role in roleManager.Roles.ToList())
            {
                var userRole = new UserRoleViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                userRole.IsSelected = await userManager.IsInRoleAsync(user, role.Name);
                model.Add(userRole);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles(IList<UserRoleViewModel> model, string userId)
        {
            ViewBag.UserId = userId;

            var user = await userManager.FindByIdAsync(userId);
            if (user == default)
            {
                ViewBag.ErrorMsg = $"The user id {userId} cannot be found.";
                return View("Error");
            }

            var roles = await userManager.GetRolesAsync(user);

            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot remove user from roles.");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(m => m.IsSelected).Select(m => m.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add selected roles to user.");
                return View(model);
            }

            return RedirectToAction("EditUser", "Administration", new { Id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == default)
            {
                ViewBag.ErrorMsg = $"The user id {userId} cannot be found.";
                return View("Error");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimViewModel()
            {
                UserId = user.Id
            };

            foreach (var claim in ClaimStore.AllClaims)
            {
                var userClaim = new UserClaim()
                {
                    ClaimType = claim.Type
                };

                userClaim.IsSelected = existingUserClaims.Any(c => c.Type == claim.Type);
                model.UserClaims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserClaims(UserClaimViewModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == default)
            {
                ViewBag.ErrorMsg = $"The user id {userId} cannot be found.";
                return View("Error");
            }

            var claims = await userManager.GetClaimsAsync(user);

            var result = await userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot remove user claims.");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user, model.UserClaims.Where(uc => uc.IsSelected).Select(uc => new Claim(uc.ClaimType, uc.ClaimType)));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add selected claims to user.");
                return View(model);
            }

            return RedirectToAction("EditUser", "Administration", new { Id = user.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == default)
            {
                ViewBag.ErrorMsg = $"The user id {id} cannot be found.";
                return View("Error");
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers", "Administration");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View("ListUsers");
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
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
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
                    ModelState.AddModelError(error.Code, error.Description);
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

            var model = new EditRoleViewModel()
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
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
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
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoleUsers(string roleId)
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
        public async Task<IActionResult> EditRoleUsers(IList<RoleUserViewModel> model, string roleId)
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
                        //    ModelState.AddModelError(error.Code, $"User '{user.UserName}' : {error.Description}");
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

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == default)
            {
                ViewBag.ErrorMsg = $"The role id {id} cannot be found.";
                return View("Error");
            }

            try
            {
                //throw new Exception("Some error occured.");
                var result = await roleManager.DeleteAsync(role);   //DbUpdateException

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            catch (DbUpdateException ex)
            {
                logger.LogError($"Error while deleting role : {ex}");
                ViewBag.ErrorTitle = $"Role {role.Name} is in use.";
                ViewBag.ErrorMsg = $"Role {role.Name} cannot be deleted as there are Users in this role. If you want to delete the role, try removing the users from the role first and then try deleting the role again.";
                return View("Exception");
            }
            // or first check if any user exist for the given role and list them
            return View("ListRoles");
        }
    }
}
