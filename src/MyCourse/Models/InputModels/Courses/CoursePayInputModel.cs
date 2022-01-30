namespace MyCourse.Models.InputModels.Courses;

public class CoursePayInputModel
{
    public int CourseId { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public Money Price { get; set; }
    public string ReturnUrl { get; set; }
    public string CancelUrl { get; set; }
}
