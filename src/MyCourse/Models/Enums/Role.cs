using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models.Enums;

public enum Role
{
    [Display(Name = "Amministratore")]
    Administrator,
    [Display(Name = "Docente")]
    Teacher
}
