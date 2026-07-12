using Bogus;
using FluentAssertions;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Tests.Domain.User.ValueObjects;

public class EmailAddressTests
{
    private readonly Faker _faker = new("pt_BR");

    [Fact]
    public void Create_ShouldThrowException_WhenEmailIsEmpty()
    {
        Action act = () => EmailAddress.Create(string.Empty);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("E-mail é obrigatório.");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenEmailDoesNotContainAt()
    {
        var invalidEmail = $"{_faker.Internet.UserName()}example.com";

        Action act = () => EmailAddress.Create(invalidEmail);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("E-mail inválido: precisa conter @.");
    }

    [Fact]
    public void Create_ShouldReturnTrimmedEmail_WhenEmailIsValid()
    {
        var expectedEmail = _faker.Internet.Email();
        var rawEmail = $"  {expectedEmail}  ";

        var email = EmailAddress.Create(rawEmail);

        email.Should().NotBeNull();
        email.Value.Should().Be(expectedEmail);
        email.ToString().Should().Be(expectedEmail);
    }
}
