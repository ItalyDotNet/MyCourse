using System;

namespace MyCourse.Models.Exceptions.Application
{
    public class CourseTitleUnavailableException : Exception
    {
        public CourseTitleUnavailableException(string title, Exception innerException) : base($"Course title '{title}' existed", innerException)
        {
        }
    }
}