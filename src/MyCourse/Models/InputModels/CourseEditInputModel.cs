using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Http;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.InputModels
{
    public class CourseEditInputModel
    {
        public int Id { get; set; }

        [Display(Name = "Titolo")]
        public string Title { get; set; }
        
        [Display(Name = "Descrizione")]
        public string Description { get; set; }
        
        [Display(Name = "Immagine rappresentativa")]
        public string ImagePath { get; set; }

        [Display(Name = "Email di contatto")]
        public string Email { get; set; }

        [Display(Name = "Prezzo intero")]
        public Money FullPrice { get; set; }

        [Display(Name = "Prezzo corrente")]
        public Money CurrentPrice { get; set; }

        [Display(Name = "Nuova immagine...")]
        public IFormFile Image { get; set; }

        public static CourseEditInputModel FromDataRow(DataRow courseRow)
        {
            var courseEditInputModel = new CourseEditInputModel
            {
                Title = Convert.ToString(courseRow["Title"]),
                Description = Convert.ToString(courseRow["Description"]),
                ImagePath = Convert.ToString(courseRow["ImagePath"]),
                Email = Convert.ToString(courseRow["Email"]),
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
            return courseEditInputModel;
        }

        public static CourseEditInputModel FromEntity(Course course)
        {
            return new CourseEditInputModel {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Email = course.Email,
                ImagePath = course.ImagePath,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice
            };
        }
    }
}