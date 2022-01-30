using Microsoft.AspNetCore.Http;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.Services.Application
{
    public interface IErrorViewSelectorService
    {
        ErrorViewData GetErrorViewData(HttpContext context);
    }
}