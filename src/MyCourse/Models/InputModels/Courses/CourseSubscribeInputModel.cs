using System;
using System.ComponentModel.DataAnnotations;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.InputModels.Courses
{
    public class CourseSubscribeInputModel
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public Money Paid { get; set; }
        public string TransactionId { get; set; }
    }
}