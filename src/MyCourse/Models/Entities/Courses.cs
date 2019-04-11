using System;
using System.Collections.Generic;

namespace MyCourse.Models.Entities
{
    public partial class Courses
    {
        public Courses()
        {
            Lessons = new HashSet<Lessons>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public string FullPriceAmount { get; set; }
        public string FullPriceCurrency { get; set; }
        public string CurrentPriceAmount { get; set; }
        public string CurrentPriceCurrency { get; set; }

        public virtual ICollection<Lessons> Lessons { get; set; }
    }
}
