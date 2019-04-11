using System;
using System.Collections.Generic;

namespace MyCourse.Models.Entities
{
    public partial class Lessons
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }

        public virtual Courses Course { get; set; }
    }
}
