using System.Collections.Generic;
using MyCourse.Models.InputModels;

namespace MyCourse.Models.ViewModels
{
    public class CourseListViewModel
    {
        public List<CourseViewModel> Courses { get; set; }
        public CourseListInputModel Input { get; set; }
    }
}