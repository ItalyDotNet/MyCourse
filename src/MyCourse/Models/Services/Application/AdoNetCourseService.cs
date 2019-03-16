using System;
using System.Collections.Generic;
using System.Data;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly IDatabaseAccessor db;
        public AdoNetCourseService(IDatabaseAccessor db)
        {
            this.db = db;
        }
        public CourseDetailViewModel GetCourse(int id)
        {
            string query = "SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id=" + id +
            "; SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId=" + id;
            DataSet dataSet = db.Query(query);

            //Course
            var courseTable = dataSet.Tables[0];
            if (courseTable.Rows.Count != 1) {
                throw new InvalidOperationException($"Did not return exactly 1 row for Course {id}");
            }
            var courseRow = courseTable.Rows[0];
            var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

            //Course lessons
            var lessonDataTable = dataSet.Tables[1];

            foreach(DataRow lessonRow in lessonDataTable.Rows) {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
            }
            return courseDetailViewModel; 
        }

        public List<CourseViewModel> GetCourses()
        {
            string query = "SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses";
            DataSet dataSet = db.Query(query);
            var dataTable = dataSet.Tables[0];
            var courseList = new List<CourseViewModel>();
            foreach(DataRow courseRow in dataTable.Rows) {
                CourseViewModel courseViewModel = CourseViewModel.FromDataRow(courseRow);
                courseList.Add(courseViewModel);
            }
            return courseList;
        }
    }
}