﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ClaimStore
    {
        public static IList<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "false"),
            new Claim("Edit Role", "false"),
            new Claim("Delete Role", "false")
        };
    }
}
