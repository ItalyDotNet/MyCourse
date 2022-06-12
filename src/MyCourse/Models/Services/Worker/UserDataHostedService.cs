using System.IO.Compression;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using MyCourse.Controllers;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels.Courses;
using MyCourse.Models.ViewModels.Lessons;

namespace MyCourse.Models.Services.Worker;

public class UserDataHostedService : BackgroundService, IUserDataService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IEmailClient emailClient;
    private readonly LinkGenerator linkGenerator;
    private readonly IHostEnvironment env;
    private readonly ILogger<UserDataHostedService> logger;
    private readonly BufferBlock<string> queue = new();
    public UserDataHostedService(IServiceProvider serviceProvider,
                                 IEmailClient emailClient,
                                 LinkGenerator linkGenerator,
                                 IHostEnvironment env,
                                 ILogger<UserDataHostedService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.emailClient = emailClient;
        this.linkGenerator = linkGenerator;
        this.env = env;
        this.logger = logger;
    }

    public void EnqueueUserDataDownload(string userId)
    {
        queue.Post(userId);
        int i = queue.GetHashCode();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string userId = null;
            try
            {
                int a = queue.GetHashCode();
                userId = await queue.ReceiveAsync(stoppingToken);
                
                using IServiceScope scope = serviceProvider.CreateScope();

                // Otteniamo l'ApplicationUser
                // Recuperare i dati dell'utente e creare il file zip
                // Inviare il link all'utente via email

                ApplicationUser user = await GetUserAsync(scope.ServiceProvider, userId);
                string zipFileUrl = await CreateZipFile(user, scope.ServiceProvider, stoppingToken);
                await EmailZipFileLinkToUser(user, zipFileUrl, stoppingToken);
            }
            catch(Exception exc)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(exc, "Error while preparing data for user {userId}", userId);
                }
            }
        }
    }

    private async Task<ApplicationUser> GetUserAsync(IServiceProvider serviceProvider, string userId)
    {
        UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser user = await userManager.FindByIdAsync(userId);
        return user;
    }

    private async Task EmailZipFileLinkToUser(ApplicationUser user, string zipFileUrl, CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        await emailClient.SendEmailAsync(user.Email, "I tuoi corsi", $"Il file zip contenente i dati dei corsi video è pronto. Lo puoi scaricare da <a href=\"{zipFileUrl}\">{zipFileUrl}</a>");
    }

    private async Task<string> CreateZipFile(ApplicationUser user, IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        Guid zipFileId = Guid.NewGuid();
        string zipFilePath = GetUserDataZipFileLocation(user.Id, zipFileId);
        
        ICourseService courseService = serviceProvider.GetRequiredService<ICourseService>();      
        ILessonService lessonService = serviceProvider.GetRequiredService<ILessonService>();
        List<CourseDetailViewModel> courses = await courseService.GetCoursesByAuthorAsync(user.Id);

        using Stream file = File.OpenWrite(zipFilePath);
        using ZipArchive zip = new(file, ZipArchiveMode.Create);
        foreach (CourseDetailViewModel courseDetail in courses)
        {
            stoppingToken.ThrowIfCancellationRequested();

            await AddZipEntry(zip, "Corso.txt", $"{courseDetail.Title}\r\n{courseDetail.Description}");

            foreach (LessonViewModel lesson in courseDetail.Lessons)
            {
                stoppingToken.ThrowIfCancellationRequested();

                LessonDetailViewModel lessonDetail = await lessonService.GetLessonAsync(lesson.Id);
                await AddZipEntry(zip, $"Lezioni/{lessonDetail.Id}.txt", $"{lessonDetail.Title}\r\n{lessonDetail.Description}");
            }
        }

        IServer server = serviceProvider.GetRequiredService<IServer>();
        IServerAddressesFeature feature = server.Features.Get<IServerAddressesFeature>();
        Uri serverUri = new Uri(feature.Addresses.First());

        string zipDownloadUrl = linkGenerator.GetUriByAction(action: nameof(UserDataController.Download), controller: "UserData", values: new { id = zipFileId }, scheme: serverUri.Scheme, host: new HostString(serverUri.Host, serverUri.Port));
        return zipDownloadUrl;
    }

    private async Task AddZipEntry(ZipArchive zip, string entryName, string entryContent)
    {
        ZipArchiveEntry entry = zip.CreateEntry(entryName, CompressionLevel.NoCompression);
        using StreamWriter streamWriter = new(entry.Open());
        await streamWriter.WriteAsync(entryContent);
    }

    public string GetUserDataZipFileLocation(string userId, Guid zipFileId)
    {
        string zipFileName = $"{userId}_{zipFileId}.zip";
        string zipRootDirectoryPath = GetZipRootDirectoryPath();
        string zipFilePath = Path.Combine(zipRootDirectoryPath, zipFileName);
        return zipFilePath;
    }

    public IEnumerable<string> EnumerateAllUserDataZipFileLocations()
    {
        string zipRootDirectoryPath = GetZipRootDirectoryPath();
        return Directory.EnumerateFiles(zipRootDirectoryPath, "*.zip");
    }

    private string GetZipRootDirectoryPath()
    {
        return Path.Combine(env.ContentRootPath, "Downloads");
    }
}
