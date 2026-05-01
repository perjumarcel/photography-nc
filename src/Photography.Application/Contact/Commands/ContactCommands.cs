using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photography.Application.Common.Email;
using Photography.Application.Contact.Dtos;
using Photography.SharedKernel;
using System.Net.Mail;

namespace Photography.Application.Contact.Commands;

public sealed record SendContactMessageCommand(ContactMessageDto Dto) : IRequest<Result>;

/// <summary>
/// Options controlling where contact-form notifications are delivered.
/// Bound from the <c>Contact</c> configuration section.
/// </summary>
public sealed class ContactOptions
{
    public const string SectionName = "Contact";

    /// <summary>
    /// Recipient for contact-form notifications. When unset, the handler falls back to
    /// <c>Email:AdminRecipient</c> (read directly from configuration by the host) and,
    /// failing that, structured-logs the submission only.
    /// </summary>
    public string? NotificationRecipient { get; set; }
}

/// <summary>
/// Handles a public contact form submission. Validates the payload, structured-logs it,
/// and (when a recipient is configured) dispatches a notification email via
/// <see cref="IEmailSender"/>. Email failures are logged but do NOT fail the request —
/// the user has done nothing wrong if our SMTP relay is down, and we do not want to
/// expose infrastructure errors to anonymous visitors.
/// </summary>
public sealed class SendContactMessageHandler(
    ILogger<SendContactMessageHandler> logger,
    IEmailSender emailSender,
    IOptions<ContactOptions> contactOptions)
    : IRequestHandler<SendContactMessageCommand, Result>
{
    private const int MaxNameLength = 128;
    private const int MaxEmailLength = 256;
    private const int MaxPhoneLength = 64;
    private const int MaxMetadataLength = 256;
    private const int MaxMessageLength = 4000;

    private readonly ContactOptions _contact = contactOptions.Value;

    public async Task<Result> Handle(SendContactMessageCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        if (!string.IsNullOrWhiteSpace(dto.Website))
            return Result.Fail("Invalid message");

        var name = dto.Name?.Trim();
        var email = dto.Email?.Trim();
        var phone = Normalize(dto.Phone);
        var eventType = Normalize(dto.EventType);
        var preferredDate = Normalize(dto.PreferredDate);
        var venue = Normalize(dto.Venue);
        var budget = Normalize(dto.EstimatedBudgetRange);
        var sourcePage = Normalize(dto.SourcePage);
        var message = dto.Message?.Trim();

        if (string.IsNullOrWhiteSpace(name) || name.Length > MaxNameLength)
            return Result.Fail("Invalid name");
        if (string.IsNullOrWhiteSpace(email) || email.Length > MaxEmailLength || !IsValidEmail(email))
            return Result.Fail("Invalid email");
        if (phone is { Length: > MaxPhoneLength })
            return Result.Fail("Invalid phone");
        if (!IsValidMetadata(eventType) || !IsValidMetadata(preferredDate) || !IsValidMetadata(venue) ||
            !IsValidMetadata(budget) || !IsValidMetadata(sourcePage))
            return Result.Fail("Invalid message details");
        if (string.IsNullOrWhiteSpace(message) || message.Length > MaxMessageLength)
            return Result.Fail("Invalid message");

        logger.LogInformation(
            "Contact message received from {ContactName} <{ContactEmail}>. Phone: {ContactPhone}. Event: {EventType}. PreferredDate: {PreferredDate}. Venue: {Venue}. Budget: {Budget}. Source: {SourcePage}. Message: {ContactMessage}",
            name, email, phone, eventType, preferredDate, venue, budget, sourcePage, message);

        var recipient = _contact.NotificationRecipient;
        if (!string.IsNullOrWhiteSpace(recipient))
        {
            var subject = $"Contact form: {name}";
            var body =
                $"From: {name} <{email}>{Environment.NewLine}" +
                FormatLine("Phone", phone) +
                FormatLine("Event/session type", eventType) +
                FormatLine("Preferred date", preferredDate) +
                FormatLine("Venue/location", venue) +
                FormatLine("Estimated budget", budget) +
                FormatLine("Source page", sourcePage) +
                $"{Environment.NewLine}" +
                message;

            try
            {
                await emailSender.SendAsync(
                    new EmailMessage(To: recipient, Subject: subject, Body: body, ReplyTo: email),
                    ct);
            }
            catch (Exception ex)
            {
                // Don't fail the public endpoint — the message is in the log already.
                logger.LogError(ex, "Failed to deliver contact-form notification to {Recipient}", recipient);
            }
        }

        return Result.Ok();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var parsed = new MailAddress(email);
            return string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsValidMetadata(string? value)
        => value is null || value.Length <= MaxMetadataLength;

    private static string FormatLine(string label, string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : $"{label}: {value}{Environment.NewLine}";
}
