using Bogus;
using FluentAssertions;
using Moq;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Commands.CreateCampaign;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Models;

namespace SolidarityConnection.Tests.Application.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandHandlerTests
{
    private readonly Faker _faker = new("pt_BR");
    private readonly Mock<ICampaignRepository> _campaignRepositoryMock;
    private readonly CreateCampaignCommandHandler _handler;

    public CreateCampaignCommandHandlerTests()
    {
        _campaignRepositoryMock = new Mock<ICampaignRepository>();
        _handler = new CreateCampaignCommandHandler(_campaignRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPersistCampaignAndReturnMappedOutput()
    {
        Campaign? persistedCampaign = null;
        var command = new CreateCampaignCommand(
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
    }
}