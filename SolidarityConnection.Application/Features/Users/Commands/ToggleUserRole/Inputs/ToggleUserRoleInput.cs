namespace SolidarityConnection.Application.Features.Users.Commands.ToggleUserRole.Inputs;

public class ToggleUserRoleInput
{
    public Guid PublicId { get; set; }

    public ToggleUserRoleCommand MapToCommand()
    {
        return new ToggleUserRoleCommand(PublicId);
    }
}
