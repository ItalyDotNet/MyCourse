using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.InputModels.Courses;

public class CourseVoteInputModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int Vote { get; set; }
}
