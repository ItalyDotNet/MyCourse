namespace MyCourse.Models.Services.Worker;

public class ClearDataHostedService : BackgroundService
{
    private readonly IUserDataService userDataService;
    private readonly ILogger<ClearDataHostedService> logger;

    public ClearDataHostedService(IUserDataService userDataService, ILogger<ClearDataHostedService> logger)
    {
        this.userDataService = userDataService;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DateTime expirationDate = DateTime.Now.AddDays(-7);
                foreach (string zipFile in userDataService.EnumerateAllUserDataZipFileLocations())
                {
                    FileInfo fileInfo = new(zipFile);
                    if (fileInfo.CreationTime < expirationDate)
                    {
                        fileInfo.Delete();
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception exc)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(exc, "Si è verificato un errore durante l'eliminazione dei vecchi file Zip");
                }
            }
        }
    }
}