using Bogus;
using FluentAssertions;
using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Exceptions;
using CampaignEntity = SolidarityConnection.Domain.Campaign.Models.Campaign;

namespace SolidarityConnection.Tests.Domain.Campaign.Models;

public class CampaignTests
{
    private readonly Faker _faker = new("pt_BR");

    [Fact]
    public void Create_ShouldReturnCampaignWithExpectedProperties()
    {
        var title = _faker.Lorem.Sentence(3);
        var description = _faker.Lorem.Paragraph();
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = startDate.AddDays(30);
        var targetAmount = _faker.Random.Decimal(100, 10000);
        var managerId = _faker.Random.Int(1, 100);

        var campaign = CampaignEntity.Create(title, description, startDate, endDate, targetAmount, managerId);

        campaign.Should().NotBeNull();
        campaign.Id.Should().NotBeEmpty();
        campaign.Title.Should().Be(title.Trim());
        campaign.Description.Should().Be(description.Trim());
        campaign.StartDate.Should().Be(startDate);
        campaign.EndDate.Should().Be(endDate);
        campaign.TargetAmount.Should().Be(targetAmount);
        campaign.AmountRaised.Should().Be(0);
        campaign.Status.Should().Be(CampaignStatus.Active);
        campaign.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateEditableProperties()
    {
        var campaign = CampaignEntity.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            500,
            1);

        campaign.AddDonation(120);

        var newTitle = _faker.Lorem.Sentence(4);
        var newDescription = _faker.Lorem.Paragraph();
        var newStartDate = DateTime.UtcNow.Date.AddDays(2);
        var newEndDate = DateTime.UtcNow.Date.AddDays(20);
        var newTargetAmount = 1500m;

        campaign.UpdateDetails(newTitle, newDescription, newStartDate, newEndDate, newTargetAmount);

        campaign.Title.Should().Be(newTitle.Trim());
        campaign.Description.Should().Be(newDescription.Trim());
        campaign.StartDate.Should().Be(newStartDate);
        campaign.EndDate.Should().Be(newEndDate);
        campaign.TargetAmount.Should().Be(newTargetAmount);
        campaign.AmountRaised.Should().Be(120);
        campaign.ManagerId.Should().Be(1);
    }

    [Fact]
    public void PauseCampaign_ShouldThrowException_WhenCampaignIsClosed()
    {
        var campaign = CampaignEntity.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            500,
            1);

        campaign.CloseCampaign();

        Action act = () => campaign.PauseCampaign();

        act.Should()
            .Throw<CampaignDomainException>()
            .WithMessage("Uma campanha encerrada não pode ser pausada.");
    }

    [Fact]
    public void AddDonation_ShouldIncreaseAmountRaised_WhenAmountIsValid()
    {
        var campaign = CampaignEntity.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            1000,
            1);

        campaign.AddDonation(150.75m);

        campaign.AmountRaised.Should().Be(150.75m);
    }

    [Fact]
    public void AddDonation_ShouldThrowException_WhenAmountIsInvalid()
    {
        var campaign = CampaignEntity.Create(
            _faker.Lorem.Sentence(3),
            _faker.Lorem.Paragraph(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(10),
            1000,
            1);

        Action act = () => campaign.AddDonation(0);

        act.Should()
            .Throw<CampaignDomainException>()
            .WithMessage("O valor da doação deve ser maior que zero.");
    }
}