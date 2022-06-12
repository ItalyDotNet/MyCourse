using Microsoft.AspNetCore.Identity.UI.Services;

namespace MyCourse.Models.Services.Infrastructure;

public interface IEmailClient : IEmailSender
{
    Task SendEmailAsync(string recipientEmail, string replyToEmail, string subject, string htmlMessage, CancellationToken token = default);
}
