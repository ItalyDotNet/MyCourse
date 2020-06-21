using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.InputModels.Lessons
{
    public class LessonCreateInputModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio"),
         MinLength(5, ErrorMessage = "Il titolo dev'essere di almeno {1} caratteri"),
         MaxLength(100, ErrorMessage = "Il titolo dev'essere di al massimo {1} caratteri"),
         RegularExpression(@"^[0-9A-z\u00C0-\u00ff\s\.']+$", ErrorMessage = "Titolo non valido")] //Questa espressione regolare include anche i caratteri accentati
        public string Title { get; set; }
    }
}