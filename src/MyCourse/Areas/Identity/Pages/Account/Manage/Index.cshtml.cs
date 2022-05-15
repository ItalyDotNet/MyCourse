using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyCourse.Areas.Identity.Pages.Account.Manage;

public partial class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public string Username { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public DateTimeOffset EcommerceConsent { get; set; }
    public DateTimeOffset? NewsletterConsent { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Il nome completo è obbligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Il nome completo deve essere di almeno {2} e di al massimo {1} caratteri.")]
        [Display(Name = "Nome completo")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Deve essere un numero di telefono valido")]
        [Display(Name = "Numero di telefono")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Iscrizione alla newsletter")]
        public bool NewsletterConsent { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Username = userName;

        EcommerceConsent = user.EcommerceConsent;
        NewsletterConsent = user.NewsletterConsent;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber,
            NewsletterConsent = NewsletterConsent is not null,
            FullName = user.FullName
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Non è stato possibile trovare il profilo utente con ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Non è stato possibile trovare il profilo utente con ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        user.FullName = Input.FullName;
        if (Input.NewsletterConsent)
        {
            if (user.NewsletterConsent is null)
            {
                user.NewsletterConsent = DateTimeOffset.Now;
            }
        }
        else
        {
            user.NewsletterConsent = null;
        }

        IdentityResult result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            StatusMessage = "Si è verificato un errore imprevisto nel salvere il profilo dell'utente.";
            return Page();
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Si è verificato un errore imprevisto nell'impostare il numero di telefono.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Il tuo profilo è stato aggiornato";
        return RedirectToPage();
    }
}
