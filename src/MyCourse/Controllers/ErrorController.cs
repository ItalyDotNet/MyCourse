using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Exceptions;

namespace MyCourse.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            switch(feature.Error)
            {
                case CourseNotFoundException exc:
                    ViewData["Title"] = "Corso non trovato";
                    Response.StatusCode = 404;
                    return View("CourseNotFound");

                default:
                    ViewData["Title"] = "Errore";
                    return View();
            }
        }
    }
}