using FluentAssertions;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Tests.Domain.User.ValueObjects;

public class RawPasswordTests
{
    [Fact]
    public void Create_ShouldThrowException_WhenPasswordIsTooShort()
    {
        Action act = () => RawPassword.Create("Abc@123");

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("A senha deve ter no mínimo 8 caracteres.");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPasswordHasNoUppercaseLetter()
    {
        Action act = () => RawPassword.Create("senha@123");

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("A senha deve ter no mínimo 1 letra maiúscula.");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPasswordHasNoSpecialCharacter()
    {
        Action act = () => RawPassword.Create("Senha1234");

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("A senha deve ter no mínimo 1 caractere especial.");
    }

    [Fact]
    public void Create_ShouldReturnPassword_WhenPasswordIsValid()
    {
        const string validPassword = "Senha@123";

        var password = RawPassword.Create(validPassword);

        password.Should().NotBeNull();
        password.Value.Should().Be(validPassword);
    }
}
