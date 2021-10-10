using System;

namespace MyCourse.Models.Exceptions.Application
{
    public class CourseSubscriptionNotFoundException : Exception
    {
        public CourseSubscriptionNotFoundException(int courseId) : base($"Subscription to course {courseId} not found")
        {
        }
    }
}