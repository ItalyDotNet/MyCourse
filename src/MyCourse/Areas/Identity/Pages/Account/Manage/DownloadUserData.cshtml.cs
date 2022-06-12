using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyCourse.Models.Services.Worker;

namespace MyCourse.Areas.Identity.Pages.Account.Manage;

[Authorize(Roles = nameof(Role.Teacher))]
public class DownloadUserDataModel : PageModel
{
    private readonly IUserDataService _userDataService;

    public DownloadUserDataModel(
        IUserDataService userDataService,
        ILogger<DownloadPersonalDataModel> logger)
    {
        _userDataService = userDataService;
    }

    public IActionResult OnPostAsync()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _userDataService.EnqueueUserDataDownload(userId);

        return Page();
    }
}