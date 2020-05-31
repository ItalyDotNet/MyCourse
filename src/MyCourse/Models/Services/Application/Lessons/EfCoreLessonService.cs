using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCourse.Models.Entities;
using MyCourse.Models.Exceptions.Application;
using MyCourse.Models.InputModels.Lessons;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels.Lessons;

namespace MyCourse.Models.Services.Application.Lessons
{
    public class EfCoreLessonService : ILessonService
    {
        private readonly ILogger<EfCoreLessonService> logger;
        private readonly MyCourseDbContext dbContext;

        public EfCoreLessonService(ILogger<EfCoreLessonService> logger, MyCourseDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<LessonDetailViewModel> GetLessonAsync(int id)
        {
            IQueryable<LessonDetailViewModel> queryLinq = dbContext.Lessons
                .AsNoTracking()
                .Where(lesson => lesson.Id == id)
                .Select(lesson => LessonDetailViewModel.FromEntity(lesson)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            LessonDetailViewModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                logger.LogWarning("Lesson {id} not found", id);
                throw new LessonNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<LessonEditInputModel> GetLessonForEditingAsync(int id)
        {
            IQueryable<LessonEditInputModel> queryLinq = dbContext.Lessons
                .AsNoTracking()
                .Where(lesson => lesson.Id == id)
                .Select(lesson => LessonEditInputModel.FromEntity(lesson)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            LessonEditInputModel inputModel = await queryLinq.FirstOrDefaultAsync();

            if (inputModel == null)
            {
                logger.LogWarning("Lesson {id} not found", id);
                throw new LessonNotFoundException(id);
            }

            return inputModel;
        }

        public async Task<LessonDetailViewModel> CreateLessonAsync(LessonCreateInputModel inputModel)
        {
            var lesson = new Lesson(inputModel.Title, inputModel.CourseId);
            dbContext.Add(lesson);
            await dbContext.SaveChangesAsync();

            return LessonDetailViewModel.FromEntity(lesson);
        }

        public async Task<LessonDetailViewModel> EditLessonAsync(LessonEditInputModel inputModel)
        {
            Lesson lesson = await dbContext.Lessons.FindAsync(inputModel.Id);
            
            if (lesson == null)
            {
                throw new LessonNotFoundException(inputModel.Id);
            }

            lesson.ChangeTitle(inputModel.Title);
            lesson.ChangeDescription(inputModel.Description);
            lesson.ChangeDuration(inputModel.Duration);
            lesson.ChangeOrder(inputModel.Order);
            dbContext.Entry(lesson).Property(lesson => lesson.RowVersion).OriginalValue = inputModel.RowVersion;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OptimisticConcurrencyException();
            }

            return LessonDetailViewModel.FromEntity(lesson);
        }
    }
}