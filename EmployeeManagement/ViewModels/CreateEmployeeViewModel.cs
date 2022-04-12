using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class CreateEmployeeViewModel
    {
        [Required]
        [MaxLength(64, ErrorMessage = "Name cannot exceed 64 characters.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters.")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public IFormFile Photo { get; set; } //use IList<IFormFile> for multiple
    }
}
