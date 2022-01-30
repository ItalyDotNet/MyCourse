using System;
using System.Collections.Generic;
using System.Data;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueObjects;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Extensions
{
    public static class DataRowExtensions
    {
        public static CourseDetailViewModel ToCourseDetailViewModel(this DataRow dataRow)
        {
            var courseDetailViewModel = new CourseDetailViewModel
            {
                Title = Convert.ToString(dataRow["Title"]),
                Description = Convert.ToString(dataRow["Description"]),
                ImagePath = Convert.ToString(dataRow["ImagePath"]),
                Author = Convert.ToString(dataRow["Author"]),
                Rating = Convert.ToDouble(dataRow["Rating"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dataRow["FullPrice_Currency"])),
                    Convert.ToDecimal(dataRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dataRow["CurrentPrice_Currency"])),
                    Convert.ToDecimal(dataRow["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(dataRow["Id"]),
                Lessons = new List<LessonViewModel>()
            };
            return courseDetailViewModel;
        }

        public static CourseViewModel ToCourseViewModel(this DataRow courseRow)
        {
            var courseViewModel = new CourseViewModel {
                Title = Convert.ToString(courseRow["Title"]),
                ImagePath = Convert.ToString(courseRow["ImagePath"]),
                Author = Convert.ToString(courseRow["Author"]),
                Rating = Convert.ToDouble(courseRow["Rating"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["FullPrice_Currency"])),
                    Convert.ToDecimal(courseRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["CurrentPrice_Currency"])),
                    Convert.ToDecimal(courseRow["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(courseRow["Id"])
            };
            return courseViewModel;
        }

        public static LessonViewModel ToLessonViewModel(this DataRow dataRow)
        {
            var lessonViewModel = new LessonViewModel {
                Id = Convert.ToInt32(dataRow["Id"]),
                Title = Convert.ToString(dataRow["Title"]),
                Description = Convert.ToString(dataRow["Description"]),
                Duration = TimeSpan.Parse(Convert.ToString(dataRow["Duration"])),
            };
            return lessonViewModel;
        }
    }
}