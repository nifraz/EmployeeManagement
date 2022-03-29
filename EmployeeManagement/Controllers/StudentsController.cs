using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class StudentsController : Controller
    {
        public string List()
        {
            return "List() from StudentsController.";
        }

        public string Details()
        {
            return "Details() from StudentsController.";
        }
    }
}
