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

            var employeeDetailsViewModel = new EmployeeDetailsViewModel()
            {
                Employee = employeeRepository.Get(id ?? 1),
                PageTitle = "Employee Details"
            };
            return View(employeeDetailsViewModel);
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
                string uniqueFileName = ProcessUploadedImage(model);

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

        private string ProcessUploadedImage(EmployeeCreateViewModel model)
        {
            string uniqueFileName = default;
            if (model.Photo != default)
            {
                var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Photo.FileName);
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Photo.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        [HttpGet]
        public ViewResult Edit(int? id)
        {
            var employee = employeeRepository.Get(id ?? 1);
            var employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoName = employee.PhotoName
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = employeeRepository.Get(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != default)
                {
                    if (model.ExistingPhotoName != default)
                    {
                        var photoPath = Path.Combine(webHostEnvironment.WebRootPath, "images", model.ExistingPhotoName);
                        System.IO.File.Delete(photoPath);
                    }
                    employee.PhotoName = ProcessUploadedImage(model);
                }

                employeeRepository.Update(employee);
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
