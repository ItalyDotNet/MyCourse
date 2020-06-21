using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.InputModels.Courses
{
    public class CourseDeleteInputModel
    {
        [Required]
        public int Id { get; set; }
    }
}