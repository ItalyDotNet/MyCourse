using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.ViewModels.Courses;

namespace MyCourse.Models.Services.Application.Courses
{
    public interface ICourseService
    {
        Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model);
        Task<CourseDetailViewModel> GetCourseAsync(int id);
        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();
        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();
        Task<CourseEditInputModel> GetCourseForEditingAsync(int id);
        Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel);
        Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel);
        Task DeleteCourseAsync(CourseDeleteInputModel inputModel);
        Task<bool> IsTitleAvailableAsync(string title, int excludeId);
        Task<string> GetCourseAuthorIdAsync(int courseId);
        Task SendQuestionToCourseAuthorAsync(int courseId, string question);
        Task<int> GetCourseCountByAuthorIdAsync(string authorId);
        Task SubscribeCourseAsync(CourseSubscribeInputModel inputModel);
        Task<bool> IsCourseSubscribedAsync(int courseId, string userId);
        Task<string> GetPaymentUrlAsync(int courseId);
        Task<CourseSubscribeInputModel> CapturePaymentAsync(int courseId, string token);
        Task<int?> GetCourseVoteAsync(int courseId);
        Task VoteCourseAsync(CourseVoteInputModel inputModel);
    }
}