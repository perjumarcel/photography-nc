using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Photography.Application.Common.Email;
using Photography.Application.Contact.Commands;
using Photography.Application.Contact.Dtos;
using Photography.SharedKernel;
using Xunit;

namespace Photography.Application.Tests.Contact;

public class SendContactMessageHandlerTests
{
    private static SendContactMessageHandler Build(
        IEmailSender? sender = null,
        string? recipient = null)
    {
        var options = Options.Create(new ContactOptions { NotificationRecipient = recipient });
        return new SendContactMessageHandler(
            NullLogger<SendContactMessageHandler>.Instance,
            sender ?? Mock.Of<IEmailSender>(),
            options);
    }

    [Fact]
    public async Task Accepts_Well_Formed_Message()
    {
        var dto = new ContactMessageDto("Jane", "jane@example.com", "Hello, please contact me about a wedding shoot.");
        var result = await Build().Handle(new SendContactMessageCommand(dto), CancellationToken.None);
        Assert.True(result.Success);
    }

    [Theory]
    [InlineData("", "j@e.com", "msg")]
    [InlineData("Jane", "no-at-symbol", "msg")]
    [InlineData("Jane", "jane@example.com<script>", "msg")]
    [InlineData("Jane", "j@e.com", "")]
    public async Task Rejects_Invalid_Payloads(string name, string email, string message)
    {
        var result = await Build().Handle(
            new SendContactMessageCommand(new ContactMessageDto(name, email, message)),
            CancellationToken.None);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Rejects_Honeypot_Submissions_Without_Sending_Email()
    {
        var sender = new Mock<IEmailSender>(MockBehavior.Strict);
        var sut = Build(sender.Object, recipient: "owner@example.com");

        var dto = new ContactMessageDto("Jane", "jane@example.com", "Hi", Website: "https://spam.example");
        var result = await sut.Handle(new SendContactMessageCommand(dto), CancellationToken.None);

        Assert.False(result.Success);
        sender.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Rejects_Oversized_Message()
    {
        var huge = new string('x', 5000);
        var result = await Build().Handle(
            new SendContactMessageCommand(new ContactMessageDto("J", "j@e.com", huge)),
            CancellationToken.None);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Sends_Notification_When_Recipient_Configured()
    {
        var sender = new Mock<IEmailSender>();
        sender
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = Build(sender.Object, recipient: "owner@example.com");

        var dto = new ContactMessageDto(
            "  Jane Doe  ",
            " jane@example.com ",
            "  Need a portrait session.  ",
            Phone: " +373 68 538 087 ",
            EventType: "Portrait",
            PreferredDate: "2026-06-20",
            Venue: "Chisinau",
            EstimatedBudgetRange: "Premium collection",
            SourcePage: "/contact");
        var result = await sut.Handle(new SendContactMessageCommand(dto), CancellationToken.None);

        Assert.True(result.Success);
        sender.Verify(s => s.SendAsync(
            It.Is<EmailMessage>(m =>
                m.To == "owner@example.com" &&
                m.ReplyTo == "jane@example.com" &&
                m.Subject.Contains("Jane Doe") &&
                m.Body.Contains("+373 68 538 087") &&
                m.Body.Contains("Portrait") &&
                m.Body.Contains("2026-06-20") &&
                m.Body.Contains("Chisinau") &&
                m.Body.Contains("Premium collection") &&
                m.Body.Contains("/contact") &&
                m.Body.Contains("Need a portrait session.")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Rejects_Oversized_Metadata()
    {
        var huge = new string('x', 300);
        var result = await Build().Handle(
            new SendContactMessageCommand(new ContactMessageDto("Jane", "jane@example.com", "Hi", Venue: huge)),
            CancellationToken.None);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Does_Not_Send_When_Recipient_Not_Configured()
    {
        var sender = new Mock<IEmailSender>(MockBehavior.Strict);

        var sut = Build(sender.Object, recipient: null);

        var dto = new ContactMessageDto("Jane", "jane@example.com", "Hi");
        var result = await sut.Handle(new SendContactMessageCommand(dto), CancellationToken.None);

        Assert.True(result.Success);
        sender.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Swallows_Email_Failures_So_Caller_Always_Succeeds()
    {
        var sender = new Mock<IEmailSender>();
        sender
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("smtp down"));

        var sut = Build(sender.Object, recipient: "owner@example.com");

        var dto = new ContactMessageDto("Jane", "jane@example.com", "Hi");
        var result = await sut.Handle(new SendContactMessageCommand(dto), CancellationToken.None);

        Assert.True(result.Success);
    }
}
