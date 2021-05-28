using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MyCourse.Models.Services.Application.Courses;

namespace MyCourse.Models.Authorization
{
    public class CourseAuthorRequirementHandler : AuthorizationHandler<CourseAuthorRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICachedCourseService courseService;

        public CourseAuthorRequirementHandler(IHttpContextAccessor httpContextAccessor, ICachedCourseService courseService)
        {
            this.courseService = courseService;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                             CourseAuthorRequirement requirement)
        {
            // 1. Leggere l'id dell'utente dalla sua identità
            string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Capire a quale corso sta cercando di accedere
            int courseId = context.Resource is int ? (int)context.Resource : // context.Resource è il valore passato come secondo argomento da IAuthorizationService.AuthorizeAsync(User, course.Id, "NomePolicy")
                           Convert.ToInt32(httpContextAccessor.HttpContext.Request.RouteValues["id"]);
            if (courseId == 0)
            {
                context.Fail();
                return;
            }

            // 3. Estrarre dal database l'id dell'autore del corso
            string authorId = await courseService.GetCourseAuthorIdAsync(courseId);

            // 4. Verificare che l'id dell'utente sia uguale all'id dell'autore del corso
            if (userId == authorId)
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