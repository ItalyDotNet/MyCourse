using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MyCourse.Models.InputModels.Courses;

namespace MyCourse.Models.Services.Infrastructure
{
    public class LocalTransactionLogger : ITransactionLogger
    {
        private readonly IHostEnvironment env;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public LocalTransactionLogger(IHostEnvironment env)
        {
            this.env = env;
        }

        public async Task LogTransactionAsync(CourseSubscribeInputModel inputModel)
        {
            string filePath = Path.Combine(env.ContentRootPath, "Data", "transactions.txt");
            string content = $"\r\n{inputModel.TransactionId}\t{inputModel.PaymentDate}\t{inputModel.PaymentType}\t{inputModel.Paid}\t{inputModel.UserId}\t{inputModel.CourseId}";
            try
            {
                await semaphore.WaitAsync();
                await File.AppendAllTextAsync(filePath, content);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}