using System.Text.Json.Serialization;

namespace SolidarityConnection.Domain.Donation.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DonationStatus
{
    Pending = 0,
    Paid = 1,
    Rejected = 2
}
