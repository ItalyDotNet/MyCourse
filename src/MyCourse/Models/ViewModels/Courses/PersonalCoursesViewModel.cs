using System.Data;

namespace MyCourse.Models.ViewModels.Courses;

public class PersonalCoursesViewModel
{
    public List<CourseViewModel> AuthoredCourses { get; set; }
    public List<CourseViewModel> SubscribedCourses { get; set; }
}
