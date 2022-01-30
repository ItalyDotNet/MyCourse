using System;
using System.Data;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public Money FullPrice { get; set; }
        public Money CurrentPrice { get; set; }

        public static CourseViewModel FromDataRecord(IDataRecord dataRecord)
        {
            var courseViewModel = new CourseViewModel {
                Title = Convert.ToString(dataRecord["Title"]),
                ImagePath = Convert.ToString(dataRecord["ImagePath"]),
                Author = Convert.ToString(dataRecord["Author"]),
                Rating = Convert.ToDouble(dataRecord["Rating"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dataRecord["FullPrice_Currency"])),
                    Convert.ToDecimal(dataRecord["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dataRecord["CurrentPrice_Currency"])),
                    Convert.ToDecimal(dataRecord["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(dataRecord["Id"])
            };
            return courseViewModel;
        }

        public static CourseViewModel FromEntity(Course course)
        {
            return new CourseViewModel {
                Id = course.Id,
                Title = course.Title,
                ImagePath = course.ImagePath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice
            };
        }
    }
}