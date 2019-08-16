using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class MemoryCacheCourseService : ICachedCourseService
    {
        private readonly ICourseService courseService;
        private readonly IMemoryCache memoryCache;
        public MemoryCacheCourseService(ICourseService courseService, IMemoryCache memoryCache)
        {
            this.courseService = courseService;
            this.memoryCache = memoryCache;
        }

        //TODO: ricordati di usare memoryCache.Remove($"Course{id}") quando aggiorni il corso

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            return memoryCache.GetOrCreateAsync($"Course{id}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); //Esercizio: provate a recuperare il valore 60 usando il servizio di configurazione
                return courseService.GetCourseAsync(id);
            });
        }

        public Task<List<CourseViewModel>> GetCoursesAsync(string search, int page, string orderby, bool ascending)
        {
            return memoryCache.GetOrCreateAsync($"Courses{search}-{page}-{orderby}-{ascending}", cacheEntry => 
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return courseService.GetCoursesAsync(search, page, orderby, ascending);
            });
        }
    }
}