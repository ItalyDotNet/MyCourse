using System;

namespace MyCourse.Models.Exceptions.Application
{
    public class LessonNotFoundException : Exception
    {
        public LessonNotFoundException(int lessonId) : base($"Lesson {lessonId} not found")
        {
        }
    }
}