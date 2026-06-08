using SolidarityConnection.Domain.Campaign.Enums;
using SolidarityConnection.Domain.Campaign.Exceptions;
using System;

namespace SolidarityConnection.Domain.Campaign.Models;

public class Campaign
{
    private Campaign()
    {
    }

    private Campaign(
        string title,
        string description,
        DateTime startDate,
        DateTime endDate,
        decimal targetAmount,
        int managerId)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateDates(startDate, endDate);
        ValidateTargetAmount(targetAmount);
        ValidateManagerId(managerId);

        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        StartDate = startDate;
        EndDate = endDate;
        TargetAmount = targetAmount;
        AmountRaised = 0m;
        Status = CampaignStatus.Active;
        ManagerId = managerId;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal TargetAmount { get; private set; }
    public decimal AmountRaised { get; private set; }
    public CampaignStatus Status { get; private set; }
    public int ManagerId { get; private set; }

    public static Campaign Create(
        string title,
        string description,
        DateTime startDate,
        DateTime endDate,
        decimal targetAmount,
        int managerId)
    {
        return new Campaign(title, description, startDate, endDate, targetAmount, managerId);
    }

    public void CloseCampaign()
    {
        Status = CampaignStatus.Closed;
    }

    public void PauseCampaign()
    {
        if (Status == CampaignStatus.Closed)
        {
            throw new CampaignDomainException("A closed campaign cannot be paused.");
        }

        Status = CampaignStatus.Paused;
    }

    public void AddDonation(decimal amount)
    {
        if (amount <= 0)
        {
            throw new CampaignDomainException("Donation amount must be greater than zero.");
        }

        AmountRaised += amount;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new CampaignDomainException("Campaign title is required.");
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new CampaignDomainException("Campaign description is required.");
        }
    }

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        var utcToday = DateTime.UtcNow.Date;

        if (endDate.Date < utcToday)
        {
            throw new CampaignDomainException("Campaign end date cannot be in the past.");
        }

        if (endDate <= startDate)
        {
            throw new CampaignDomainException("Campaign end date must be greater than the start date.");
        }
    }

    private static void ValidateTargetAmount(decimal targetAmount)
    {
        if (targetAmount <= 0)
        {
            throw new CampaignDomainException("Campaign target amount must be greater than zero.");
        }
    }

    private static void ValidateManagerId(int managerId)
    {
        if (managerId <= 0)
        {
            throw new CampaignDomainException("Campaign manager id is required.");
        }
    }
}
