using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Application.Courses;
using MyCourse.Models.ViewModels.Courses;
using MyCourse.Models.ViewModels.Home;

namespace MyCourse.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index([FromServices] ICachedCourseService courseService)
        {
            ViewData["Title"] = "Benvenuto su MyCourse!";
            List<CourseViewModel> bestRatingCourses = await courseService.GetBestRatingCoursesAsync();
            List<CourseViewModel> mostRecentCourses = await courseService.GetMostRecentCoursesAsync();

            HomeViewModel viewModel = new()
            {
                BestRatingCourses = bestRatingCourses,
                MostRecentCourses = mostRecentCourses
            };

            return View(viewModel);
        }
    }
}