using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MyCourse.Models.Services.Application.Courses;
using MyCourse.Models.Services.Application.Lessons;

namespace MyCourse.Models.Authorization
{
    public class CourseAuthorRequirementHandler : AuthorizationHandler<CourseAuthorRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICachedCourseService courseService;
        private readonly ILessonService lessonService;

        public CourseAuthorRequirementHandler(IHttpContextAccessor httpContextAccessor, ICachedCourseService courseService, ILessonService lessonService)
        {
            this.courseService = courseService;
            this.lessonService = lessonService;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                     CourseAuthorRequirement requirement)
        {
            string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int courseId;
            if (context.Resource is int)
            {
                courseId = (int)context.Resource;
            }
            else
            {
                int id = Convert.ToInt32(httpContextAccessor.HttpContext.Request.RouteValues["id"]);
                if (id == 0)
                {
                    context.Fail();
                    return;
                }

                // A quale controller sto cercando di accedere?
                switch (httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString().ToLowerInvariant())
                {
                    // Si tratta di una lezione. Otteniamo l'id del corso a cui appartiene
                    case "lessons":
                        courseId = (await lessonService.GetLessonAsync(id)).CourseId;
                        break;

                    // L'id era proprio quello di un corso
                    case "courses":
                        courseId = id;
                        break;

                    default:
                        // Controller non supportato
                        context.Fail();
                        return;
                }
            }

            string authorId = await courseService.GetCourseAuthorIdAsync(courseId);
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