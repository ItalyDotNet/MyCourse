using System.Net;

namespace MyCourse.Models.ValueObjects
{
    public class ErrorViewData
    {
        public ErrorViewData(
            string title,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string viewName = "Index")
        {
            Title = title;
            StatusCode = statusCode;
            ViewName = viewName;
        }

        public string Title { get; }
        public HttpStatusCode StatusCode { get; }
        public string ViewName { get; }
    }
}