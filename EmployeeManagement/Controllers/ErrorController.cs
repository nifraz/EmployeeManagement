using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

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
            //ViewBag.Path = statusCodeResult.OriginalPath;
            //ViewBag.QryStr = statusCodeResult.OriginalQueryString;
            logger.LogWarning($"{statusCode} {ViewBag.ErrorMsg} Path: {statusCodeResult.OriginalPath} QueryString: {statusCodeResult.OriginalQueryString}");
            return View("Error");
        }

        [Route("Error")]
        public IActionResult ExceptionHandler()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            //ViewBag.Msg = error.Error.Message;
            //ViewBag.Path = error.Path;
            //ViewBag.StackTrace = error.Error.StackTrace;
            logger.LogError($"Path: {error.Path} threw an Exception: {error.Error}");

            return View("Exception");
        }
    }
}
