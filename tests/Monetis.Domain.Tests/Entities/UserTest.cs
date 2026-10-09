using FluentAssertions;
using Monetis.Domain.Entities;
using Xunit;

namespace Monetis.Domain.Tests.Entities;

public class UserTest
{
    private sealed class UserTestable : User
    {
        public UserTestable() : base() { }
    }

    [Fact]
    public void ShouldAllowInstantiationThroughProtectedConstructor()
    {
        var user = new UserTestable();
        user.Should().NotBeNull();
    }
}
