using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.InputModels.Courses
{
    public class CourseSubscriptionViewModel
    {
        public string Title { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public Money Paid { get; set; }
        public string TransactionId { get; set; }

        public static CourseSubscriptionViewModel FromDataRow(DataRow dataRow)
        {
            return new CourseSubscriptionViewModel
            {
                Title = Convert.ToString(dataRow["Title"]),
                PaymentDate = Convert.ToDateTime(dataRow["PaymentDate"]),
                PaymentType = Convert.ToString(dataRow["PaymentType"]),
                Paid = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dataRow["Paid_Currency"])),
                    Convert.ToDecimal(dataRow["Paid_Amount"])
                ),
                TransactionId = Convert.ToString(dataRow["TransactionId"])
            };
        }

        public static CourseSubscriptionViewModel FromEntity(Subscription subscription)
        {
            return new CourseSubscriptionViewModel
            {
                Title = subscription.Course.Title,
                Paid = subscription.Paid,
                PaymentDate = subscription.PaymentDate,
                PaymentType = subscription.PaymentType,
                TransactionId = subscription.TransactionId
            };
        }
    }
}