using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Exceptions.Application;
using MyCourse.Models.Exceptions.Infrastructure;

namespace MyCourse.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            switch(feature.Error)
            {
                case CourseNotFoundException exc:
                    ViewData["Title"] = "Corso non trovato";
                    Response.StatusCode = 404;
                    return View("CourseNotFound");

                case CourseSubscriptionException exc:
                    ViewData["Title"] = "Non è stato possibile iscriverti al corso";
                    Response.StatusCode = 400;
                    return View();

                case PaymentGatewayException exc:
                    ViewData["Title"] = "Si è verificato un errore nel pagamento";
                    Response.StatusCode = 400;
                    return View();

                case UserUnknownException exc:
                    ViewData["Title"] = "Utente sconosciuto";
                    Response.StatusCode = 400;
                    return View();

                case SendException exc:
                    ViewData["Title"] = "Non è stato possibile inviare il messaggio, riprova più tardi";
                    Response.StatusCode = 500;
                    return View();

                default:
                    ViewData["Title"] = "Errore";
                    return View();
            }
        }
    }
}