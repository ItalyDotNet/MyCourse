namespace MyCourse.Models.Exceptions.Infrastructure;

public class ConstraintViolationException : Exception
{
    public ConstraintViolationException(Exception innerException) : base($"A violation occurred for a database constraint", innerException)
    {
    }
}
