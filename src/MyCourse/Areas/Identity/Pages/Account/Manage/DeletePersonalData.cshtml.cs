using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyCourse.Areas.Identity.Pages.Account.Manage;

public class DeletePersonalDataModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<DeletePersonalDataModel> _logger;

    public DeletePersonalDataModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<DeletePersonalDataModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Non è stato possibile trovare il profilo utente con ID '{_userManager.GetUserId(User)}'.");
        }
        
        IList<Claim> claims = await _userManager.GetClaimsAsync(user);
        if (claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == nameof(Role.Teacher)))
        {
            TempData["ConfirmationMessage"] = $"Non puoi eliminare il tuo account autonomamente, contatta il nostro supporto.";
            return RedirectToPage("PersonalData");
        }

        RequirePassword = await _userManager.HasPasswordAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Non è stato possibile trovare il profilo utente con ID '{_userManager.GetUserId(User)}'.");
        }

        RequirePassword = await _userManager.HasPasswordAsync(user);
        if (RequirePassword)
        {
            if (!await _userManager.CheckPasswordAsync(user, Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Password incorretta.");
                return Page();
            }
        }

        var result = await _userManager.DeleteAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Si è verificato un errore inatteso nel tentativo di eliminare il profilo utente con ID '{userId}'.");
        }

        await _signInManager.SignOutAsync();

        _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        return Redirect("~/");
    }
}
