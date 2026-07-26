using Bogus;
using FluentAssertions;
using Moq;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Commands.CreateOrUpdateCampaign;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Tests.Application.Campaigns.Commands.CreateOrUpdateCampaign;

public class CreateOrUpdateCampaignCommandHandlerTests
{
    private readonly Faker _faker = new("pt_BR");
    private readonly Mock<ICampaignRepository> _campaignRepositoryMock;
    private readonly Mock<ICampaignTransparencyWriter> _transparencyWriterMock;
    private readonly CreateOrUpdateCampaignCommandHandler _handler;

    public CreateOrUpdateCampaignCommandHandlerTests()
    {
        _campaignRepositoryMock = new Mock<ICampaignRepository>();
        _transparencyWriterMock = new Mock<ICampaignTransparencyWriter>();
        _handler = new CreateOrUpdateCampaignCommandHandler(_campaignRepositoryMock.Object, _transparencyWriterMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPersistCampaignAndReturnMappedOutput()
    {
        Campaign? persistedCampaign = null;
        var command = new CreateOrUpdateCampaignCommand(
            CampaignId: null,
            Title: _faker.Lorem.Sentence(3),
            Description: _faker.Lorem.Paragraph(),
            StartDate: DateTime.UtcNow.Date.AddDays(1),
            EndDate: DateTime.UtcNow.Date.AddDays(20),
            TargetAmount: _faker.Random.Decimal(100, 5000),
            Status: CampaignStatus.Active,
            ManagerId: _faker.Random.Int(1, 50));

        _campaignRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()))
            .Callback<Campaign, CancellationToken>((campaign, _) => persistedCampaign = campaign)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Description.Should().Be(command.Description);
        result.Data.TargetAmount.Should().Be(command.TargetAmount);
        result.Data.ManagerId.Should().Be(command.ManagerId);
        result.Data.Status.Should().Be("Active");

        persistedCampaign.Should().NotBeNull();
        persistedCampaign!.Title.Should().Be(command.Title);

        _campaignRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _campaignRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingCampaign_WhenCampaignIdIsProvided()
    {
        Campaign? updatedCampaign = null;
        var existingCampaign = Campaign.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            _faker.Random.Decimal(100, 5000),
            _faker.Random.Int(1, 50));

        var command = new CreateOrUpdateCampaignCommand(
            CampaignId: existingCampaign.Id,
            Title: _faker.Lorem.Sentence(4),
            Description: _faker.Lorem.Paragraph(),
            StartDate: DateTime.UtcNow.Date.AddDays(2),
            EndDate: DateTime.UtcNow.Date.AddDays(25),
            TargetAmount: _faker.Random.Decimal(1000, 7000),
            Status: CampaignStatus.Paused,
            ManagerId: _faker.Random.Int(51, 100));

        _campaignRepositoryMock
            .Setup(repository => repository.GetByIdAsync(existingCampaign.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCampaign);

        _campaignRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()))
            .Callback<Campaign, CancellationToken>((campaign, _) => updatedCampaign = campaign)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(existingCampaign.Id);
        result.Data.Title.Should().Be(command.Title);
        result.Data.Description.Should().Be(command.Description);
        result.Data.StartDate.Should().Be(command.StartDate);
        result.Data.EndDate.Should().Be(command.EndDate);
        result.Data.TargetAmount.Should().Be(command.TargetAmount);
        result.Data.Status.Should().Be("Paused");
        result.Data.ManagerId.Should().Be(existingCampaign.ManagerId);

        updatedCampaign.Should().NotBeNull();
        updatedCampaign!.Id.Should().Be(existingCampaign.Id);
        updatedCampaign.Title.Should().Be(command.Title);
        updatedCampaign.Status.Should().Be(CampaignStatus.Paused);

        _campaignRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _campaignRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCampaignIdDoesNotExist()
    {
        var command = new CreateOrUpdateCampaignCommand(
            CampaignId: Guid.NewGuid(),
            Title: _faker.Lorem.Sentence(3),
            Description: _faker.Lorem.Paragraph(),
            StartDate: DateTime.UtcNow.Date.AddDays(1),
            EndDate: DateTime.UtcNow.Date.AddDays(20),
            TargetAmount: _faker.Random.Decimal(100, 5000),
            Status: CampaignStatus.Active,
            ManagerId: _faker.Random.Int(1, 50));

        _campaignRepositoryMock
            .Setup(repository => repository.GetByIdAsync(command.CampaignId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Campaign?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Campanha não encontrada.");
        result.Data.Should().BeNull();

        _campaignRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _campaignRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _transparencyWriterMock.Verify(
            writer => writer.UpsertCampaignAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}