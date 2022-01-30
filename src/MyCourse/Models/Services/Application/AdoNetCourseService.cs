using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions;
using MyCourse.Models.InputModels;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ValueObjects;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly ILogger<AdoNetCourseService> logger;
        private readonly IDatabaseAccessor db;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;
        public AdoNetCourseService(ILogger<AdoNetCourseService> logger, IDatabaseAccessor db, IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            this.coursesOptions = coursesOptions;
            this.logger = logger;
            this.db = db;
        }
        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            logger.LogInformation("Course {id} requested", id);

            //Course
            FormattableString courseQuery = $"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id}";
            IAsyncEnumerable<IDataRecord> courseResults = db.QueryAsync(courseQuery);
            CourseDetailViewModel courseDetailViewModel = null;
            await foreach(IDataRecord dataRecord in courseResults)
            {
                courseDetailViewModel = CourseDetailViewModel.FromDataRecord(dataRecord);
                break;
            }
            if (courseDetailViewModel == null)
            {
                logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            //Course lessons
            FormattableString lessonsQuery = $"SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId={id}";
            IAsyncEnumerable<IDataRecord> lessonsResults = db.QueryAsync(lessonsQuery);
            await foreach (IDataRecord dataRecord in lessonsResults)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRecord(dataRecord);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
            }
            return courseDetailViewModel;
        }        
        public async Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Rating",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

                ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
                return result.Results;
        }
        
        public async Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Id",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

                ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
                return result.Results;
        }

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            string orderby = model.OrderBy == "CurrentPrice" ? "CurrentPrice_Amount" : model.OrderBy;
            string direction = model.Ascending ? "ASC" : "DESC";
                                    
            //Pagina di corsi
            var courseList = new List<CourseViewModel>();
            FormattableString coursesQuery = $"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Title LIKE {"%" + model.Search + "%"} ORDER BY {(Sql) orderby} {(Sql) direction} LIMIT {model.Limit} OFFSET {model.Offset}";
            IAsyncEnumerable<IDataRecord> coursesResults = db.QueryAsync(coursesQuery);
            await foreach (IDataRecord dataRecord in coursesResults)
            {
                CourseViewModel courseViewModel = CourseViewModel.FromDataRecord(dataRecord);
                courseList.Add(courseViewModel);
            }

            //Conteggio totale dei corsi
            int count = 0;
            FormattableString countQuery = $"SELECT COUNT(*) FROM Courses WHERE Title LIKE {"%" + model.Search + "%"}";
            IAsyncEnumerable<IDataRecord> countResults = db.QueryAsync(countQuery);
            await foreach (IDataRecord dataRecord in countResults)
            {
                count = dataRecord.GetInt32(0);
                break;
            }

            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>
            {
                Results = courseList,
                TotalCount = count
            };

            return result;
        }
    }
}