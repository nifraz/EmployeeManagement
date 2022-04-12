using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public IList<string> Users { get; set; }

        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
    }
}
