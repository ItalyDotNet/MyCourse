using System;

namespace MyCourse.Models.Exceptions
{
    public class CourseImageInvalidException : Exception
    {
        public CourseImageInvalidException(int courseId, Exception innerException) : base($"Image for course '{courseId}' is not valid", innerException)
        {
        }
    }
}