namespace MyCourse.Models.Exceptions.Application;

public class CourseSubscriptionException : Exception
{
    public CourseSubscriptionException(int courseId) : base($"Could not subscribe to course {courseId}")
    {
    }
}
