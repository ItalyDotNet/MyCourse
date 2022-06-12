namespace MyCourse.Models.Services.Worker;

public interface IUserDataService
{
    void EnqueueUserDataDownload(string userId);
    string GetUserDataZipFileLocation(string userId, Guid downloadFileId);
    IEnumerable<string> EnumerateAllUserDataZipFileLocations();
}
