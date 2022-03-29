using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        //[Route("~/")]
        //[Route("")]
        //[Route("~/Home")]
        public ViewResult Index()
        {
            return View(_employeeRepository.GetAll());
        }

        //[Route("{id?}")]
        public ViewResult Details(int? id)
        {
            //var emp = _employeeRepository.Get(1);
            //return View("~/MyViews/Test.cshtml"); //absolute path requires extension
            //return View("../../MyViews/Test");  //relative path does not require extension

            //using ViewData
            //ViewData["PageTitle"] = "Employee Details";
            //ViewData["Employee"] = emp;
            //return View("DetailsViewData"); 

            //using ViewBag
            //ViewBag.PageTitle = "Employee Details";
            //ViewBag.Employee = emp;
            //return View("DetailsViewBag");

            //using strongly typed view
            //return View(emp);

            ///// using strongly typed viewmodel

            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.Get(id ?? 1),
                PageTitle = "Employee Details"
            };
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeRepository.Add(employee);
                //return RedirectToAction("Details", new { employee.Id });
            }

            return View();
        }

        // for API use ObjectResult or JsonResult

        //public ObjectResult Details()
        //{
        //    var emp = _employeeRepository.Get(1);
        //    return new ObjectResult(emp);
        //}

        //public JsonResult Index(int id)
        //{
        //    return Json(_employeeRepository.Get(id));
        //}
    }
}
