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
        public AuthorizePolicyAttribute(Policy policy)
        {
            // Poi convertiamo il nome della policy in formato stringa,
            // così come richiede la proprietà Policy dell'AuthorizeAttribute
            Policy = policy.ToString();
        }
    }
}
