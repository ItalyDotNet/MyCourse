using System;
using System.Data;
using MyCourse.Models.Entities;

namespace MyCourse.Models.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }

        public static LessonViewModel FromDataRecord(IDataRecord dataRecord)
        {
            var lessonViewModel = new LessonViewModel
            {
                Id = Convert.ToInt32(dataRecord["Id"]),
                Title = Convert.ToString(dataRecord["Title"]),
                Description = Convert.ToString(dataRecord["Description"]),
                Duration = TimeSpan.Parse(Convert.ToString(dataRecord["Duration"])),
            };
            return lessonViewModel;
        }

        public static LessonViewModel FromEntity(Lesson lesson)
        {
            return new LessonViewModel
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Duration = lesson.Duration,
                Description = lesson.Description
            };
        }
    }
}