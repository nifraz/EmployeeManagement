using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        //[ValidEmailDomain("live.com", ErrorMessage = "Only emails from domain 'live.com' are allowed.")]
        //[Remote(action: "IsEmailAvailable", controller: "Account")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string City { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public IList<string> Claims { get; set; } = new List<string>();
    }
}
