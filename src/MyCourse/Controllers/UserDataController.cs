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
        string zipFilePath = userDataService.GetUserDataZipFileLocation(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
        FileInfo zipFileInfo = new(zipFilePath);
        if (zipFileInfo.Exists)
        {
            FileStream zipFileStream = zipFileInfo.OpenRead();
            return File(zipFileStream, "application/zip", "Corsi.zip");
        }
        
        return NotFound();
    }
}
