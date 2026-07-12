using Bogus;
using FluentAssertions;
using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.User.ValueObjects;
using UserEntity = SolidarityConnection.Domain.User.Models.User;

namespace SolidarityConnection.Tests.Domain.User.Models;

public class UserTests
{
    private const string ValidCpf = "52998224725";
    private readonly Faker _faker = new("pt_BR");

    [Fact]
    public void Create_ShouldReturnActiveUserWithExpectedProperties()
    {
        var name = _faker.Name.FullName();
        var email = EmailAddress.Create(_faker.Internet.Email());
        var cpf = CpfValidator.Create(ValidCpf);
        var passwordHash = _faker.Internet.Password();

        var user = UserEntity.Create(name, email, cpf, passwordHash, EUserRole.Doador);

        user.Should().NotBeNull();
        user.PublicId.Should().NotBeEmpty();
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.Cpf.Should().Be(cpf);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(EUserRole.Doador);
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateUserProperties()
    {
        var user = UserEntity.Create(
            _faker.Name.FullName(),
            EmailAddress.Create(_faker.Internet.Email()),
            CpfValidator.Create(ValidCpf),
            _faker.Internet.Password(),
            EUserRole.Doador);

        var newName = _faker.Name.FullName();
        var newEmail = EmailAddress.Create(_faker.Internet.Email());
        var newCpf = CpfValidator.Create("11144477735");

        user.UpdateDetails(newName, newEmail, newCpf);

        user.Name.Should().Be(newName);
        user.Email.Should().Be(newEmail);
        user.Cpf.Should().Be(newCpf);
    }

    [Fact]
    public void ToggleRole_ShouldSwitchBetweenRoles()
    {
        var user = UserEntity.Create(
            _faker.Name.FullName(),
            EmailAddress.Create(_faker.Internet.Email()),
            CpfValidator.Create(ValidCpf),
            _faker.Internet.Password(),
            EUserRole.Doador);

        user.ToggleRole();
        user.Role.Should().Be(EUserRole.GestorONG);

        user.ToggleRole();
        user.Role.Should().Be(EUserRole.Doador);
    }

    [Fact]
    public void Deactivate_ShouldSetUserAsInactive()
    {
        var user = UserEntity.Create(
            _faker.Name.FullName(),
            EmailAddress.Create(_faker.Internet.Email()),
            CpfValidator.Create(ValidCpf),
            _faker.Internet.Password(),
            EUserRole.Doador);

        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }
}
