using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeEditViewModel : CreateEmployeeViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoName { get; set; }
    }
}
