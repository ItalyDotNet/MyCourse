using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.InputModels
{
    public class CourseCreateInputModel
    {
        [Required, MaxLength(255)]
        public string Title { get; set; }
    }
}