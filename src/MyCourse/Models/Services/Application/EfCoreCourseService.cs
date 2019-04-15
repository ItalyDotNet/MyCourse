using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;

        public EfCoreCourseService(MyCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            List<CourseViewModel> courses = await dbContext.Courses.Select(course => 
            new CourseViewModel {
                Id = course.Id,
                Title = course.Title,
                ImagePath = course.ImagePath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice
            })
            .ToListAsync(); //La query al database viene inviata qui, quando manifestiamo l'intenzione di voler leggere i risultati

            return courses;
        }
    }
}