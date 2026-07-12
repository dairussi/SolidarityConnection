using Bogus;
using FluentAssertions;
using Moq;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login;
using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.User.Models;
using SolidarityConnection.Domain.User.ValueObjects;

namespace SolidarityConnection.Tests.Application.Auth.Queries.Login;

public class LoginQueryHandlerTests
{
    private const string ValidCpf = "52998224725";
    private readonly Faker _faker = new("pt_BR");
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _handler = new LoginQueryHandler(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotFound()
    {
        var query = new LoginQuery(_faker.Internet.Email(), "Senha@123");

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Usuário ou senha inválidos.");
        result.Data.Should().BeNull();
        _tokenServiceMock.Verify(
            service => service.GenerateToken(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsInactive()
    {
        var query = new LoginQuery(_faker.Internet.Email(), "Senha@123");
        var inactiveUser = CreateUser();
        inactiveUser.Deactivate();

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveUser);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Usuário ou senha inválidos.");
        result.Data.Should().BeNull();
        _passwordHasherMock.Verify(
            hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenPasswordIsInvalid()
    {
        var query = new LoginQuery(_faker.Internet.Email(), "Senha@123");
        var user = CreateUser();

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(hasher => hasher.Verify(query.Password, user.PasswordHash))
            .Returns(false);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Usuário ou senha inválidos.");
        result.Data.Should().BeNull();
        _tokenServiceMock.Verify(
            service => service.GenerateToken(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        var query = new LoginQuery(_faker.Internet.Email(), "Senha@123");
        var user = CreateUser();
        const string expectedToken = "jwt-token";

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(hasher => hasher.Verify(query.Password, user.PasswordHash))
            .Returns(true);

        _tokenServiceMock
            .Setup(service => service.GenerateToken(user.Id, user.Role.ToString()))
            .Returns(expectedToken);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be(expectedToken);
        result.Data.Name.Should().Be(user.Name);
        result.Data.Role.Should().Be(user.Role.ToString());
    }

    private User CreateUser()
    {
        var user = User.Create(
            _faker.Name.FullName(),
            EmailAddress.Create(_faker.Internet.Email()),
            CpfValidator.Create(ValidCpf),
            "hashed-password",
            EUserRole.GestorONG);

        user.Id = _faker.Random.Int(1, 1000);
        return user;
    }
}
