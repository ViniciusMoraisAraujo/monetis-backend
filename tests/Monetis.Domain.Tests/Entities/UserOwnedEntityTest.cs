using Monetis.Domain.Entities;
using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Tests.Entities;

public class UserOwnedEntityTest
{
    private sealed class TestableUserOwnedEntity : UserOwnedEntity
    {
        public TestableUserOwnedEntity() : base()
        {
        }
    }

    [Fact]
    public void SetAValidGuidUserId()
    {
        //Arrange
        var entity = new TestableUserOwnedEntity();
        var userId = Guid.NewGuid();

        //Act
        var act = () => entity.SetUser(userId);

        //Assert
        act.Should().NotThrow();
        entity.UserId.Should().Be(userId);
    }

    [Fact]
    public void SetUserWhenJaExistsUserIdLaunchException()
    {
        //Arrange
        var entity = new TestableUserOwnedEntity();
        var userId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        entity.SetUser(userId);

        //Act
        var act = () => entity.SetUser(newUserId);

        //Assert
        act.Should().Throw<UserOwnedEntityUserAlreadySetException>();
        entity.UserId.Should().Be(userId);
    }

    [Fact]
    public void SetUserWhenUserIdIsEmptyGuidShouldThrowException()
    {
        // Arrange
        var entity = new TestableUserOwnedEntity();

        //Act
        var act = () => entity.SetUser(Guid.Empty);

        //Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("UserId não pode ser vazio.*")
            .And.ParamName.Should().Be("userId");
    }

    [Fact]
    public void SetUserWhenUserIdIsValidAndAlreadySetShouldNotThrowException()
    {
        // Arrange
        var entity = new TestableUserOwnedEntity();
        var userId = Guid.NewGuid();
        entity.SetUser(userId);

        //Act
        var act = () => entity.SetUser(userId);

        //Assert
        act.Should().NotThrow();
        entity.UserId.Should().Be(userId);
    }

    [Fact]
    public void ShouldValidateEntityCreationWhenUserIdIsEmpty()
    {
        //Arrange
        var entity = new TestableUserOwnedEntity();

        //Act
        var userId = entity.UserId;

        //Assert
        userId.Should().Be(Guid.Empty);
    }
}


