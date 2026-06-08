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
            throw new CampaignDomainException("Uma campanha encerrada não pode ser pausada.");
        }

        Status = CampaignStatus.Paused;
    }

    public void AddDonation(decimal amount)
    {
        if (amount <= 0)
        {
            throw new CampaignDomainException("O valor da doação deve ser maior que zero.");
        }

        AmountRaised += amount;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new CampaignDomainException("O título da campanha é obrigatório.");
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new CampaignDomainException("A descrição da campanha é obrigatória.");
        }
    }

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        var utcToday = DateTime.UtcNow.Date;

        if (endDate.Date < utcToday)
        {
            throw new CampaignDomainException("A data de término da campanha não pode estar no passado.");
        }

        if (endDate <= startDate)
        {
            throw new CampaignDomainException("A data de término da campanha deve ser maior que a data de início.");
        }
    }

    private static void ValidateTargetAmount(decimal targetAmount)
    {
        if (targetAmount <= 0)
        {
            throw new CampaignDomainException("A meta de arrecadação da campanha deve ser maior que zero.");
        }
    }

    private static void ValidateManagerId(int managerId)
    {
        if (managerId <= 0)
        {
            throw new CampaignDomainException("O identificador do gestor da campanha é obrigatório.");
        }
    }
}
