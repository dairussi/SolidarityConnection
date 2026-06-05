namespace SolidarityConnection.Domain.Common.Models;
public abstract class BaseModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
}
