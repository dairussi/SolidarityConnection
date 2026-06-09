using SolidarityConnection.Domain.Common.Exceptions;

namespace SolidarityConnection.Domain.Campaign.Exceptions;

public sealed class CampaignDomainException(string message) : DomainException(message);
