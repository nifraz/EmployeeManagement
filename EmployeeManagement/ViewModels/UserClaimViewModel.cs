using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class UserClaimViewModel
    {
        public string UserId { get; set; }
        public IList<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    }
}
