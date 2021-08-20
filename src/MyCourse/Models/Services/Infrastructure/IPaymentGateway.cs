using System.Threading.Tasks;
using MyCourse.Models.InputModels.Courses;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IPaymentGateway
    {
        Task<string> GetPaymentUrlAsync(CoursePayInputModel inputModel);
        Task<CourseSubscribeInputModel> CapturePaymentAsync(string token);
    }
}