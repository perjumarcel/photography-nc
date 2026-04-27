using Microsoft.Extensions.Logging.Abstractions;
using Photography.Application.Contact.Commands;
using Photography.Application.Contact.Dtos;
using Xunit;

namespace Photography.Application.Tests.Contact;

public class SendContactMessageHandlerTests
{
    private readonly SendContactMessageHandler _sut = new(NullLogger<SendContactMessageHandler>.Instance);

    [Fact]
    public async Task Accepts_Well_Formed_Message()
    {
        var dto = new ContactMessageDto("Jane", "jane@example.com", "Hello, please contact me about a wedding shoot.");
        var result = await _sut.Handle(new SendContactMessageCommand(dto), CancellationToken.None);
        Assert.True(result.Success);
    }

    [Theory]
    [InlineData("", "j@e.com", "msg")]
    [InlineData("Jane", "no-at-symbol", "msg")]
    [InlineData("Jane", "j@e.com", "")]
    public async Task Rejects_Invalid_Payloads(string name, string email, string message)
    {
        var result = await _sut.Handle(
            new SendContactMessageCommand(new ContactMessageDto(name, email, message)),
            CancellationToken.None);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task Rejects_Oversized_Message()
    {
        var huge = new string('x', 5000);
        var result = await _sut.Handle(
            new SendContactMessageCommand(new ContactMessageDto("J", "j@e.com", huge)),
            CancellationToken.None);
        Assert.False(result.Success);
    }
}
