using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authorizationFilterContext = context.Resource as AuthorizationFilterContext;
            if (authorizationFilterContext == default)
            {
                return Task.CompletedTask;
            }

            var loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string editedAdminId = authorizationFilterContext.HttpContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin") && context.User.HasClaim("Edit Role", "true") && loggedInAdminId.ToLowerInvariant() != editedAdminId.ToLowerInvariant())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
