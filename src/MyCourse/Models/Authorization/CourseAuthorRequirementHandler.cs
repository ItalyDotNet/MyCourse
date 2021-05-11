using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyCourse.Models.Authorization
{
    public class CourseAuthorRequirementHandler : AuthorizationHandler<CourseAuthorRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CourseAuthorRequirement requirement)
        {
            // 1. Leggere l'id dell'utente dalla sua identità
            // 2. Capire a quale corso sta cercando di accedere
            // 3. Estrarre dal database l'id dell'autore del corso
            // 4. Verificare che l'id dell'utente sia uguale all'id dell'autore del corso
            bool isAuthorized = true;
            if (isAuthorized)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}