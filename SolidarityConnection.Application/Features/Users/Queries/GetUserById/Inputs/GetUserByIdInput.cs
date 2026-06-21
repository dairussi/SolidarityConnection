namespace SolidarityConnection.Application.Features.Users.Queries.GetUserById.Inputs;

public class GetUserByIdInput
{
    public Guid PublicId { get; set; }

    public GetUserByIdQuery MapToQuery()
    {
        return new GetUserByIdQuery(PublicId);
    }
}
