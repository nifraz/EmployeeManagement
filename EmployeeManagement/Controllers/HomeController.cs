using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.employeeRepository = employeeRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        //[Route("~/")]
        //[Route("")]
        //[Route("~/Home")]
        public ViewResult Index()
        {
            return View(employeeRepository.GetAll());
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
                Employee = employeeRepository.Get(id ?? 1),
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
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != default)
                {
                    var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Photo.FileName);
                    var filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    model.Photo.CopyTo(fileStream);
                }

                //for multiple
                //if (model.Photos != default && model.Photos.Count > 0)
                //{
                //    foreach (var item in model.Photos)
                //    {
                //        var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                //        uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(item.FileName);
                //        var filePath = Path.Combine(uploadFolder, uniqueFileName);

                //        using var fileStream = new FileStream(filePath, FileMode.Create);
                //        item.CopyTo(fileStream);
                //    }
                //}

                var employee = new Employee()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoName = uniqueFileName
                };

                employeeRepository.Add(employee);
                return RedirectToAction("Details", new { employee.Id });
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
