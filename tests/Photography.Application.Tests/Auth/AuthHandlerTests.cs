using Moq;
using Photography.Application.Auth;
using Photography.Application.Auth.Commands;
using Photography.Application.Common.Time;
using Photography.Core.Users;
using Photography.SharedKernel;
using Xunit;

namespace Photography.Application.Tests.Auth;

public class LoginHandlerTests
{
    private static (LoginHandler handler, Mock<IUserRepository> users, Mock<ITokenService> tokens) Build()
    {
        var users = new Mock<IUserRepository>();
        var tokens = new Mock<ITokenService>();
        var hasher = new PasswordHasher();
        var handler = new LoginHandler(users.Object, hasher, tokens.Object);

        tokens.Setup(t => t.IssueAccessToken(It.IsAny<User>()))
              .Returns(("jwt", DateTime.UtcNow.AddMinutes(30)));
        tokens.Setup(t => t.IssueRefreshToken())
              .Returns(("raw", "hash", DateTime.UtcNow.AddDays(14)));

        return (handler, users, tokens);
    }

    [Fact]
    public async Task Returns_Unauthorized_When_User_Missing()
    {
        var (handler, users, _) = Build();
        users.Setup(u => u.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);

        var result = await handler.Handle(new LoginCommand("a@b.com", "pw"), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorKind.Unauthorized, result.ErrorKind);
    }

    [Fact]
    public async Task Returns_Unauthorized_On_Wrong_Password()
    {
        var (handler, users, _) = Build();
        var user = User.Create(Guid.NewGuid(), "a@b.com", new PasswordHasher().Hash("right"));
        users.Setup(u => u.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await handler.Handle(new LoginCommand("a@b.com", "wrong"), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorKind.Unauthorized, result.ErrorKind);
    }

    [Fact]
    public async Task Issues_Tokens_And_Persists_Refresh_On_Success()
    {
        var (handler, users, tokens) = Build();
        var user = User.Create(Guid.NewGuid(), "a@b.com", new PasswordHasher().Hash("right"));
        users.Setup(u => u.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await handler.Handle(new LoginCommand("a@b.com", "right"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("jwt", result.Value!.AccessToken);
        Assert.Equal("raw", result.Value.RefreshToken);
        Assert.NotNull(user.RefreshTokenHash);
        users.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        tokens.Verify(t => t.IssueAccessToken(user), Times.Once);
    }
}

public class RefreshHandlerTests
{
    [Fact]
    public async Task Returns_Unauthorized_When_Token_Unknown()
    {
        var users = new Mock<IUserRepository>();
        var tokens = new Mock<ITokenService>();
        users.Setup(u => u.GetByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((User?)null);
        tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns("h");

        var handler = new RefreshHandler(users.Object, tokens.Object, new SystemClock());
        var result = await handler.Handle(new RefreshCommand("opaque"), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorKind.Unauthorized, result.ErrorKind);
    }

    [Fact]
    public async Task Rotates_Tokens_When_Refresh_Token_Matches()
    {
        var users = new Mock<IUserRepository>();
        var tokens = new Mock<ITokenService>();
        var clock = new SystemClock();
        var user = User.Create(Guid.NewGuid(), "a@b.com", "v1.100000.AAAA.BBBB");
        user.IssueRefreshToken("h", clock.UtcNow.AddDays(7));

        users.Setup(u => u.GetByRefreshTokenHashAsync("h", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns("h");
        tokens.Setup(t => t.IssueAccessToken(user)).Returns(("jwt2", clock.UtcNow.AddMinutes(30)));
        tokens.Setup(t => t.IssueRefreshToken()).Returns(("raw2", "h2", clock.UtcNow.AddDays(14)));

        var handler = new RefreshHandler(users.Object, tokens.Object, clock);
        var result = await handler.Handle(new RefreshCommand("opaque"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("raw2", result.Value!.RefreshToken);
        Assert.Equal("h2", user.RefreshTokenHash);
        users.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
