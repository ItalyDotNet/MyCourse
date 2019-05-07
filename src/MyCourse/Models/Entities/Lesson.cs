using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCourse.Models.Entities
{
    [Table("Lessons")]
    public partial class Lesson
    {
        public Lesson(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("A lesson must have a title");
            }
            Title = title;
        }
        [Key]
        public int Id { get; private set; }
        [ForeignKey(nameof(Course))]
        public int CourseId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public TimeSpan Duration { get; private set; } //00:00:00

        public virtual Course Course { get; set; }
    }
}
