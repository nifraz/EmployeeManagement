using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{StatusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMsg = "The requested URL cannot be found!";
                    break;
                default:
                    ViewBag.ErrorMsg = "Some error occured.";
                    break;
            }
            ViewBag.Path = statusCodeResult.OriginalPath;
            ViewBag.QryStr = statusCodeResult.OriginalQueryString;
            return View("Error");
        }

        [Route("Error")]
        public IActionResult ExceptionHandler()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.Msg = error.Error.Message;
            ViewBag.Path = error.Path;
            ViewBag.StackTrace = error.Error.StackTrace;

            return View("Exception");
        }
    }
}
