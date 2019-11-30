using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Exceptions;
using MyCourse.Models.Services.Application;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index([FromServices] IErrorViewSelectorService errorViewSelectorService)
        {
            ErrorViewData errorViewData = errorViewSelectorService.GetErrorViewData(HttpContext);
            ViewData["Title"] = errorViewData.Title;
            Response.StatusCode = (int) errorViewData.StatusCode;
            return View(errorViewData.ViewName);
        }
    }
}