using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MyCourse.Models.Enums;

namespace MyCourse.Customizations.Authorization
{
   [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        // Grazie a questo costruttore possiamo fornire i ruoli come Role anziché come stringhe
        // Esempio di utilizzo con 1 ruolo: [AuthorizeRole(Role.Teacher)]
        // Esempio di utilizzo con 2 ruoli: [AuthorizeRole(Role.Teacher, Role.Administrator)]
        public AuthorizeRoleAttribute(params Role[] roles)
        {
            // Poi li convertiamo a stringa e li separiamo con la virgola
            // così come vuole la proprietà Roles dell'attributo Authorize
            Roles = string.Join(",", roles.Select(role => role.ToString()));
        }
    }
}
