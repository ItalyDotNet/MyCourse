using System.Data;

namespace MyCourse.Models.ViewModels.Lessons;

public class LessonViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }

    public static LessonViewModel FromDataRow(DataRow dataRow)
    {
        LessonViewModel lessonViewModel = new()
        {
            Id = Convert.ToInt32(dataRow["Id"]),
            Title = Convert.ToString(dataRow["Title"]),
            Duration = TimeSpan.Parse(Convert.ToString(dataRow["Duration"])),
        };
        return lessonViewModel;
    }

    public static LessonViewModel FromEntity(Lesson lesson)
    {
        return new LessonViewModel
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Duration = lesson.Duration
        };
    }
}
