using Bogus;
using FluentAssertions;
using Moq;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Campaigns.Commands.DeleteCampaign;
using CampaignEntity = SolidarityConnection.Domain.Campaign.Models.Campaign;

namespace SolidarityConnection.Tests.Application.Campaigns.Commands.DeleteCampaign;

public class DeleteCampaignCommandHandlerTests
{
    private readonly Faker _faker = new("pt_BR");
    private readonly Mock<ICampaignRepository> _campaignRepositoryMock;
    private readonly DeleteCampaignCommandHandler _handler;

    public DeleteCampaignCommandHandlerTests()
    {
        _campaignRepositoryMock = new Mock<ICampaignRepository>();
        _handler = new DeleteCampaignCommandHandler(_campaignRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCampaignIsNotFound()
    {
        var command = new DeleteCampaignCommand(Guid.NewGuid());

        _campaignRepositoryMock
            .Setup(repository => repository.GetByIdAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaignEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Campanha não encontrada.");

        _campaignRepositoryMock.Verify(
            repository => repository.DeleteAsync(It.IsAny<CampaignEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCampaignAlreadyHasDonations()
    {
        var campaign = CampaignEntity.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            _faker.Random.Decimal(100, 5000),
            _faker.Random.Int(1, 50));

        campaign.AddDonation(150m);

        var command = new DeleteCampaignCommand(campaign.Id);

        _campaignRepositoryMock
            .Setup(repository => repository.GetByIdAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campaign);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Não é possível excluir a campanha porque já existe valor doado.");

        _campaignRepositoryMock.Verify(
            repository => repository.DeleteAsync(It.IsAny<CampaignEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteCampaign_WhenAmountRaisedIsZero()
    {
        var campaign = CampaignEntity.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            _faker.Random.Decimal(100, 5000),
            _faker.Random.Int(1, 50));

        var command = new DeleteCampaignCommand(campaign.Id);

        _campaignRepositoryMock
            .Setup(repository => repository.GetByIdAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campaign);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();

        _campaignRepositoryMock.Verify(
            repository => repository.DeleteAsync(campaign, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}