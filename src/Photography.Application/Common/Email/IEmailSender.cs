namespace Photography.Application.Common.Email;

/// <summary>
/// Plain-text email payload. Recipients are addresses (not display names) — the
/// caller is expected to pre-validate the format. <see cref="ReplyTo"/> is
/// optional and intended for messages that the operator may want to reply to
/// (e.g. contact-form notifications: the form submitter's address).
/// </summary>
public sealed record EmailMessage(
    string To,
    string Subject,
    string Body,
    string? ReplyTo = null);

/// <summary>
/// Sends transactional emails (e.g. contact-form notifications). Implementations
/// live in the Infrastructure layer. The default in non-configured environments
/// is a no-op that structured-logs the message — guaranteeing the API never
/// crashes when SMTP is not provisioned (e.g. local dev, CI, ephemeral previews).
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Delivers the message. Implementations MUST honour <paramref name="ct"/> and
    /// SHOULD treat transient failures as exceptions; the calling handler decides
    /// whether to surface the failure to the end user or swallow + log it.
    /// </summary>
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}
