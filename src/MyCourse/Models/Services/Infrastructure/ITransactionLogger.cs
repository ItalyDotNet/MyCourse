using MyCourse.Models.InputModels.Courses;

namespace MyCourse.Models.Services.Infrastructure;

public interface ITransactionLogger
{
    Task LogTransactionAsync(CourseSubscribeInputModel inputModel);
}
