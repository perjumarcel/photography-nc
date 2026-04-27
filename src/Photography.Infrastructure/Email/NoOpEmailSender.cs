using Microsoft.Extensions.Logging;
using Photography.Application.Common.Email;

namespace Photography.Infrastructure.Email;

/// <summary>
/// Default <see cref="IEmailSender"/> used when <c>Email:Provider</c> is unset
/// or anything other than <c>Smtp</c>. Structured-logs the outgoing message
/// instead of attempting to deliver it, so local/CI environments can exercise
/// the contact flow end-to-end without any SMTP credentials.
/// </summary>
internal sealed class NoOpEmailSender(ILogger<NoOpEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[Email NO-OP] To={To} Subject={Subject} ReplyTo={ReplyTo} BodyChars={BodyLength}",
            message.To, message.Subject, message.ReplyTo, message.Body?.Length ?? 0);
        return Task.CompletedTask;
    }
}
