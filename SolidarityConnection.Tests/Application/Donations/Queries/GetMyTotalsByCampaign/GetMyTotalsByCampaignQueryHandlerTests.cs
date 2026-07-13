using FluentAssertions;
using Moq;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Donations.Outputs;
using SolidarityConnection.Application.Features.Donations.Queries.GetMyTotalsByCampaign;

namespace SolidarityConnection.Tests.Application.Donations.Queries.GetMyTotalsByCampaign;

public class GetMyTotalsByCampaignQueryHandlerTests
{
    private readonly Mock<IDonationRepository> _donationRepositoryMock;
    private readonly GetMyTotalsByCampaignQueryHandler _handler;

    public GetMyTotalsByCampaignQueryHandlerTests()
    {
        _donationRepositoryMock = new Mock<IDonationRepository>();
        _handler = new GetMyTotalsByCampaignQueryHandler(_donationRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAggregatedTotalsForTheLoggedUser()
    {
        var query = new GetMyTotalsByCampaignQuery(42);
        IReadOnlyList<DonationTotalByCampaignOutput> totals =
        [
            new(Guid.NewGuid(), "Campanha do Agasalho", 150.00m),
            new(Guid.NewGuid(), "Campanha de Alimentos", 80.50m)
        ];

        _donationRepositoryMock
            .Setup(repository => repository.GetTotalsByDonorIdAsync(query.DonorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(totals);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(totals);
    }
}