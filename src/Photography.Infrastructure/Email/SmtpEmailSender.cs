using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Photography.Application.Common.Email;

namespace Photography.Infrastructure.Email;

/// <summary>
/// MailKit/MimeKit SMTP <see cref="IEmailSender"/>. Designed to be created per
/// send (registered as <c>Transient</c>) so that the underlying
/// <see cref="SmtpClient"/> isn't shared across concurrent requests; SmtpClient
/// is explicitly documented as not thread-safe.
/// </summary>
internal sealed class SmtpEmailSender(
    IOptions<EmailOptions> emailOptions,
    IOptions<SmtpOptions> smtpOptions,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _email = emailOptions.Value;
    private readonly SmtpOptions _smtp = smtpOptions.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (string.IsNullOrWhiteSpace(_email.FromAddress))
            throw new InvalidOperationException("Email:FromAddress is required when Provider=Smtp");
        if (string.IsNullOrWhiteSpace(_smtp.Host))
            throw new InvalidOperationException("Email:Smtp:Host is required when Provider=Smtp");

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_email.FromDisplayName ?? _email.FromAddress, _email.FromAddress));
        mime.To.Add(MailboxAddress.Parse(message.To));
        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
            mime.ReplyTo.Add(MailboxAddress.Parse(message.ReplyTo));
        mime.Subject = message.Subject;
        mime.Body = new TextPart("plain") { Text = message.Body ?? string.Empty };

        // Pick the right secure-socket option:
        //  * port 465 → SSL on connect (implicit TLS)
        //  * port 587 / explicit UseSsl=false → STARTTLS upgrade
        var socket = _smtp.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;

        using var client = new SmtpClient { Timeout = _smtp.TimeoutSeconds * 1000 };
        try
        {
            await client.ConnectAsync(_smtp.Host, _smtp.Port, socket, ct);
            if (!string.IsNullOrEmpty(_smtp.Username))
                await client.AuthenticateAsync(_smtp.Username, _smtp.Password ?? string.Empty, ct);
            await client.SendAsync(mime, ct);
        }
        finally
        {
            if (client.IsConnected)
                await client.DisconnectAsync(quit: true, ct);
        }

        logger.LogInformation(
            "Email sent To={To} Subject={Subject} via {Host}:{Port}",
            message.To, message.Subject, _smtp.Host, _smtp.Port);
    }
}
