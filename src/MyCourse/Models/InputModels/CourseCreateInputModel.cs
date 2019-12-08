using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.InputModels
{
    public class CourseCreateInputModel
    {
        [Required, MinLength(10), MaxLength(100), RegularExpression(@"^[\w\s\.]+$")]
        public string Title { get; set; }
    }
}