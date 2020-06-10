using System.Threading.Tasks;
using MyCourse.Models.InputModels.Lessons;
using MyCourse.Models.ViewModels.Lessons;

namespace MyCourse.Models.Services.Application.Lessons
{
    public interface ILessonService
    {
        Task<LessonDetailViewModel> GetLessonAsync(int id);
        Task<LessonEditInputModel> GetLessonForEditingAsync(int id);
        Task<LessonDetailViewModel> CreateLessonAsync(LessonCreateInputModel inputModel);
        Task<LessonDetailViewModel> EditLessonAsync(LessonEditInputModel inputModel);
        Task DeleteLessonAsync(LessonDeleteInputModel id);
    }
}