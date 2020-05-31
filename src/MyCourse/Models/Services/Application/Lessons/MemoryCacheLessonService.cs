using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MyCourse.Models.InputModels.Lessons;
using MyCourse.Models.ViewModels.Lessons;

namespace MyCourse.Models.Services.Application.Lessons
{
    public class MemoryCacheLessonService : ICachedLessonService
    {
        private readonly ILessonService lessonService;
        private readonly IMemoryCache memoryCache;

        public MemoryCacheLessonService(ILessonService lessonService, IMemoryCache memoryCache)
        {
            this.lessonService = lessonService;
            this.memoryCache = memoryCache;
        }

        public Task<LessonDetailViewModel> GetLessonAsync(int id)
        {
            return memoryCache.GetOrCreateAsync($"Lesson{id}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return lessonService.GetLessonAsync(id);
            });
        }

        public async Task<LessonDetailViewModel> CreateLessonAsync(LessonCreateInputModel inputModel)
        {
            LessonDetailViewModel viewModel = await lessonService.CreateLessonAsync(inputModel);
            memoryCache.Remove($"Course{viewModel.CourseId}");
            return viewModel;
        }

        public async Task<LessonDetailViewModel> EditLessonAsync(LessonEditInputModel inputModel)
        {
            LessonDetailViewModel viewModel = await lessonService.EditLessonAsync(inputModel);
            memoryCache.Remove($"Course{viewModel.CourseId}");
            memoryCache.Remove($"Lesson{viewModel.Id}");
            return viewModel;
        }

        public Task<LessonEditInputModel> GetLessonForEditingAsync(int id)
        {
            return lessonService.GetLessonForEditingAsync(id);
        }
    }
}