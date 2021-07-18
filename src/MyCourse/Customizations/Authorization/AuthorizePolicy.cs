using System;
using Microsoft.AspNetCore.Authorization;
using MyCourse.Models.Enums;

namespace MyCourse.Customizations.Authorization
{
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizePolicyAttribute : AuthorizeAttribute
    {
        // Grazie a questo costruttore possiamo fornire le policy come oggetti Policy anziché come stringhe
        // Esempio di utilizzo: [AuthorizePolicy(Policy.CourseAuthor)]
        public AuthorizePolicyAttribute(params Policy [] policies)
        {
            // Poi convertiamo il nome della policy in formato stringa,
            // così come richiede la proprietà Policy dell'AuthorizeAttribute
            // ATTENZIONE: ASP.NET Core normalmente non permette di indicare il nome di più di una policy
            // è possibile solo se scriviamo noi logica personalizzata: vedi la classe MultiAuthorizationPolicyProvider
            Policy = string.Join(",", policies);
        }
    }
}
