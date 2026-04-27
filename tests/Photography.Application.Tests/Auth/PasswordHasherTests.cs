using Photography.Application.Auth;
using Xunit;

namespace Photography.Application.Tests.Auth;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_Then_Verify_RoundTrips()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("correct horse battery staple");
        Assert.True(hasher.Verify(hash, "correct horse battery staple"));
        Assert.False(hasher.Verify(hash, "wrong password"));
    }

    [Fact]
    public void Hash_ProducesDifferentOutputForSamePassword()
    {
        var hasher = new PasswordHasher();
        var a = hasher.Hash("p4ssw0rd!");
        var b = hasher.Hash("p4ssw0rd!");
        Assert.NotEqual(a, b);
        Assert.True(hasher.Verify(a, "p4ssw0rd!"));
        Assert.True(hasher.Verify(b, "p4ssw0rd!"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("v1.short")]
    [InlineData("v2.100000.AAAA.BBBB")]
    public void Verify_RejectsMalformedHashes(string? hash)
    {
        var hasher = new PasswordHasher();
        Assert.False(hasher.Verify(hash!, "anything"));
    }
}
