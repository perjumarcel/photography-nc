using MediatR;
using Microsoft.Extensions.Logging;
using Photography.Application.Contact.Dtos;
using Photography.SharedKernel;

namespace Photography.Application.Contact.Commands;

public sealed record SendContactMessageCommand(ContactMessageDto Dto) : IRequest<Result>;

/// <summary>
/// Handles a public contact form submission. The legacy site had a no-op MVC
/// controller; this implementation validates the payload and structured-logs it.
/// A future enhancement is to inject an <c>IEmailSender</c> here without changing
/// the public contract.
/// </summary>
public sealed class SendContactMessageHandler(ILogger<SendContactMessageHandler> logger)
    : IRequestHandler<SendContactMessageCommand, Result>
{
    private const int MaxNameLength = 128;
    private const int MaxEmailLength = 256;
    private const int MaxMessageLength = 4000;

    public Task<Result> Handle(SendContactMessageCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > MaxNameLength)
            return Task.FromResult(Result.Fail("Invalid name"));
        if (string.IsNullOrWhiteSpace(dto.Email) || dto.Email.Length > MaxEmailLength || !dto.Email.Contains('@'))
            return Task.FromResult(Result.Fail("Invalid email"));
        if (string.IsNullOrWhiteSpace(dto.Message) || dto.Message.Length > MaxMessageLength)
            return Task.FromResult(Result.Fail("Invalid message"));

        logger.LogInformation(
            "Contact message received from {ContactName} <{ContactEmail}>: {ContactMessage}",
            dto.Name.Trim(), dto.Email.Trim(), dto.Message.Trim());

        return Task.FromResult(Result.Ok());
    }
}
