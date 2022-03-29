using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }
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
        public string PhotoPath { get; set; }
        //[Range(1, 12, ErrorMessage = "Month should be between 1 - 12.")]
        //[RegularExpression(@"/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/", ErrorMessage = "Invalid Something!")]
        //[Compare("ConfirmEmail", ErrorMessage = "Email Addresses do not match.")]
    }
}
