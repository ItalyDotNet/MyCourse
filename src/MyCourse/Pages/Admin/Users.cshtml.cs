using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyCourse.Models.InputModels.Users;

namespace MyCourse.Pages.Admin
{
    public class UsersModel : PageModel
    {
        public UserRoleInputModel Input { get; set; }
        
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Gestione utenti";
            return Page();
        }

        public async Task<IActionResult> OnPostAssignAsync()
        {
            // Verifichiamo se ModelState.IsValid è true
            // Con lo UserManager recuperiamo l'utente dal database in base all'email
            // Con lo UserManager recuperiamo gli attuali claim dell'utente
            // Verifichiamo se quell'utente ha già quel ruolo
            // Se ce l'ha diamo un errore
            // Se non ce l'ha aggiungiamo il claim del ruolo
            // Diamo conferma all'utente e lo reindirizziamo
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRevokeAsync()
        {
            // Verifichiamo se ModelState.IsValid è true
            // Con lo UserManager recuperiamo l'utente dal database in base all'email
            // Con lo UserManager recuperiamo gli attuali claim dell'utente
            // Verifichiamo se quell'utente ha già quel ruolo
            // Se non ce l'ha diamo un errore
            // Se ce l'ha rimuoviamo il claim del ruolo
            // Diamo conferma all'utente e lo reindirizziamo
            return RedirectToPage();
        }

    }
}