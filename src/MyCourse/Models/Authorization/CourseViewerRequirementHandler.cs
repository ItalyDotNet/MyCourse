using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MyCourse.Models.ViewModels.Courses;

namespace MyCourse.Models.Authorization;

public class CourseViewerRequirementHandler : AuthorizationHandler<CourseViewerRequirement>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ICachedCourseService courseService;

    public CourseViewerRequirementHandler(IHttpContextAccessor httpContextAccessor, ICachedCourseService courseService)
    {
        this.courseService = courseService;
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                         CourseViewerRequirement requirement)
    {
        string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        int courseId;
        switch (httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString().ToLowerInvariant())
        {
            // L'id era proprio quello di un corso
            case "courses":
                courseId = Convert.ToInt32(httpContextAccessor.HttpContext.Request.RouteValues["id"]);
                break;

            default:
                // Controller non supportato
                context.Fail();
                return;
        }
        
        CourseDetailViewModel viewModel = await courseService.GetCourseAsync(courseId);
        if (viewModel.Status == CourseStatus.Published || viewModel.AuthorId == userId)
        {
            context.Succeed(requirement);
            return;
        }

        bool isSubscribed = await courseService.IsCourseSubscribedAsync(courseId, userId);
        if (isSubscribed)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
