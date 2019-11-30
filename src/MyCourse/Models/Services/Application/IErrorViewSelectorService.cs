using Microsoft.AspNetCore.Http;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Services.Application
{
    public interface IErrorViewSelectorService
    {
        ErrorViewData GetErrorViewData(HttpContext context);
    }
}