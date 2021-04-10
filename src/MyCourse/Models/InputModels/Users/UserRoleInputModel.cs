using System.ComponentModel.DataAnnotations;
using MyCourse.Models.Enums;

namespace MyCourse.Models.InputModels.Users
{
	public class UserRoleInputModel
    {
        [Required(ErrorMessage = "L'indirizzo email è obbligatorio"),
         EmailAddress(ErrorMessage = "L'indirizzo email digitato non è valido"),
         Display(Name = "Indirizzo email")]
        public string Email { get; set; }
            
        [Required(ErrorMessage = "Il ruolo è obbligatorio"),
         Display(Name = "Ruolo")]
        public Role Role { get; set; }
    }
}