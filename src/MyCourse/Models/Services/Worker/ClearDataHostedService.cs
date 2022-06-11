namespace MyCourse.Models.Services.Worker;

public class ClearDataHostedService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000);
        }
    }
}