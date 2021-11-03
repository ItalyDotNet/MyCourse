using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.ViewModels;
using MyCourse.Models.ViewModels.Courses;

namespace MyCourse.Models.Services.Application.Courses
{
    public class MemoryCacheCourseService : ICachedCourseService
    {
        private readonly ICourseService courseService;
        private readonly IMemoryCache memoryCache;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MemoryCacheCourseService(ICourseService courseService, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            this.courseService = courseService;
            this.memoryCache = memoryCache;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            return memoryCache.GetOrCreateAsync($"Course{id}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return courseService.GetCourseAsync(id);
            });
        }

        public Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {
            return memoryCache.GetOrCreateAsync($"BestRatingCourses", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return courseService.GetBestRatingCoursesAsync();
            });
        }
        
        public Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            return memoryCache.GetOrCreateAsync($"MostRecentCourses", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return courseService.GetMostRecentCoursesAsync();
            });
        }

        public Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            //Metto in cache i risultati solo per le prime 5 pagine del catalogo, che reputo essere
            //le più visitate dagli utenti, e che perciò mi permettono di avere il maggior beneficio dalla cache.
            //E inoltre, metto in cache i risultati solo se l'utente non ha cercato nulla.
            //In questo modo riduco drasticamente il consumo di memoria RAM
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);
            
            //Se canCache è true, sfrutto il meccanismo di caching
            if (canCache)
            {
                return memoryCache.GetOrCreateAsync($"Courses{model.Page}-{model.OrderBy}-{model.Ascending}", cacheEntry => 
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                    return courseService.GetCoursesAsync(model);
                });
            }

            //Altrimenti uso il servizio applicativo sottostante, che recupererà sempre i valori dal database
            return courseService.GetCoursesAsync(model);
        }

        public Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            string authorId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            memoryCache.Remove($"CourseCountByAuthorId{authorId}");
            return courseService.CreateCourseAsync(inputModel);
        }

        public Task<bool> IsTitleAvailableAsync(string title, int id)
        {
            return courseService.IsTitleAvailableAsync(title, id);
        }

        public Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
        {
            return courseService.GetCourseForEditingAsync(id);
        }

        public async Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel)
        {
            CourseDetailViewModel viewModel = await courseService.EditCourseAsync(inputModel);
            memoryCache.Remove($"Course{inputModel.Id}");
            return viewModel;
        }

        public async Task DeleteCourseAsync(CourseDeleteInputModel inputModel)
        {
            await courseService.DeleteCourseAsync(inputModel);
            memoryCache.Remove($"Course{inputModel.Id}");
        }

        public Task SendQuestionToCourseAuthorAsync(int courseId, string question)
        {
            return courseService.SendQuestionToCourseAuthorAsync(courseId, question);
        }

        public Task<string> GetCourseAuthorIdAsync(int courseId)
        {
            return memoryCache.GetOrCreateAsync($"CourseAuthorId{courseId}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return courseService.GetCourseAuthorIdAsync(courseId);
            });
        }

        public Task<int> GetCourseCountByAuthorIdAsync(string authorId)
        {
            return memoryCache.GetOrCreateAsync($"CourseCountByAuthorId{authorId}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return courseService.GetCourseCountByAuthorIdAsync(authorId);
            });
        }

        public Task SubscribeCourseAsync(CourseSubscribeInputModel inputModel)
        {
            return courseService.SubscribeCourseAsync(inputModel);
        }

        public Task<bool> IsCourseSubscribedAsync(int courseId, string userId)
        {
            return courseService.IsCourseSubscribedAsync(courseId, userId);
        }

        public Task<string> GetPaymentUrlAsync(int courseId)
        {
            return courseService.GetPaymentUrlAsync(courseId);
        }

        public Task<CourseSubscribeInputModel> CapturePaymentAsync(int courseId, string token)
        {
            return courseService.CapturePaymentAsync(courseId, token);
        }

        public Task<int?> GetCourseVoteAsync(int courseId)
        {
            return courseService.GetCourseVoteAsync(courseId);
        }

        public Task VoteCourseAsync(CourseVoteInputModel inputModel)
        {
            return courseService.VoteCourseAsync(inputModel);
        }

        public Task<CourseSubscriptionViewModel> GetCourseSubscriptionAsync(int courseId)
        {
            return courseService.GetCourseSubscriptionAsync(courseId);
        }
    }
}