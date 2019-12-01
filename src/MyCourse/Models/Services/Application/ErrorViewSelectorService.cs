using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MyCourse.Models.Exceptions;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Services.Application
{
    //Questo servizio può essere registrato come singleton dalla classe Startup perché i messaggi di errore li determina
    //unicamente grazie all'HttpContext che gli viene passato come argomento del metodo GetErrorViewData.
    //Se invece avesse bisogno di accedere al database, cioè se avesse una dipendenza da un servizio infrastrutturale
    //allora è preferibile che sia registrato come transient in modo che il servizio infrastrutturale gli possa essere passato nel costruttore
    public class ErrorViewSelectorService : IErrorViewSelectorService
    {
        public ErrorViewData GetErrorViewData(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            return exception switch
            {
                null => new ErrorViewData(
                    title: $"Pagina '{context.Request.Path}' non trovata",
                    statusCode: HttpStatusCode.NotFound,
                    viewName: "NotFound"),

                CourseNotFoundException exc => new ErrorViewData( //Valorizzo tutti e 3 i parametri
                    title: $"Corso {exc.CourseId} non trovato",
                    statusCode: HttpStatusCode.NotFound,
                    viewName: "CourseNotFound"),

                _ => new ErrorViewData(title: "Errore") //Valorizzo solo il titolo, gli altri 2 parametri sono opzionali perché hanno dei default
            };
        }
    }
}