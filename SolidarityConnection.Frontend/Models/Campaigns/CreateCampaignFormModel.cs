using System.ComponentModel.DataAnnotations;

namespace SolidarityConnection.Frontend.Models.Campaigns;

public sealed class CreateCampaignFormModel : IValidatableObject
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? TargetAmount { get; set; }

    public int Status { get; set; } = 1;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult("O título da campanha é obrigatório.", [nameof(Title)]);
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            yield return new ValidationResult("A descrição da campanha é obrigatória.", [nameof(Description)]);
        }

        if (!StartDate.HasValue)
        {
            yield return new ValidationResult("Informe a data de início da campanha.", [nameof(StartDate)]);
        }

        if (!EndDate.HasValue)
        {
            yield return new ValidationResult("Informe a data de término da campanha.", [nameof(EndDate)]);
        }

        if (TargetAmount is null)
        {
            yield return new ValidationResult("Informe a meta de arrecadação da campanha.", [nameof(TargetAmount)]);
        }
        else if (TargetAmount <= 0)
        {
            yield return new ValidationResult("A meta de arrecadação da campanha deve ser maior que zero.", [nameof(TargetAmount)]);
        }

        if (Status is < 1 or > 3)
        {
            yield return new ValidationResult("Informe um status inicial válido para a campanha.", [nameof(Status)]);
        }

        if (StartDate.HasValue && EndDate.HasValue)
        {
            var startDate = StartDate.Value.Date;
            var endDate = EndDate.Value.Date;
            var utcToday = DateTime.UtcNow.Date;

            if (endDate < utcToday)
            {
                yield return new ValidationResult("A data de término da campanha não pode estar no passado.", [nameof(EndDate)]);
            }

            if (endDate <= startDate)
            {
                yield return new ValidationResult("A data de término da campanha deve ser maior que a data de início.", [nameof(EndDate)]);
            }
        }
    }
}