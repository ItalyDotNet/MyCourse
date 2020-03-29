using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IImageValidator
    {
        Task<bool> IsAppropriateAsync(IFormFile formFile);
    }
}