using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MyCourse.Models.Services.Application.Courses;

namespace MyCourse.Models.Authorization
{
    public class CourseLimitRequirementHandler : AuthorizationHandler<CourseLimitRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICachedCourseService courseService;

        public CourseLimitRequirementHandler(IHttpContextAccessor httpContextAccessor, ICachedCourseService courseService)
        {
            this.courseService = courseService;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                             CourseLimitRequirement requirement)
        {
            // 1. Leggere l'id dell'utente dalla sua identità
            string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Estrarre dal database i corsi creati dall'utente
            int courseCount = await courseService.GetCourseCountByAuthorIdAsync(userId);

            // 3. Verificare che il numero di corsi sia minore o uguale al limite
            if (courseCount <= requirement.Limit)
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