using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.Enums;

public enum CourseStatus
{
    [Display(Name = "Bozza")]
    Draft,
    
    [Display(Name = "Pubblico")]
    Published,

    [Display(Name = "Eliminato")]
    Deleted
}
