namespace MyCourse.Models.Exceptions.Application;

public class CourseDeletionException : Exception
{
    public CourseDeletionException(int courseId) : base($"Cannot delete course {courseId} since it has subscribers")
    {
    }
}
