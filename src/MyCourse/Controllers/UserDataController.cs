using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Services.Worker;

namespace MyCourse.Controllers;

public class UserDataController : Controller
{
    private readonly IUserDataService userDataService;

    public UserDataController(IUserDataService userDataService)
    {
        this.userDataService = userDataService;
    }

    [Authorize]
    public IActionResult Download(Guid id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string zipFilePath = userDataService.GetUserDataZipFileLocation(userId, id);
        if (System.IO.File.Exists(zipFilePath))
        {
            return PhysicalFile(zipFilePath, "application/zip", "Corsi.zip");
        }
        else
        {
            return NotFound();
        }
    }
}
