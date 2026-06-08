namespace SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged.Inputs;

public class GetUsersPagedInput
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetUsersPagedQuery MapToQuery()
    {
        return new GetUsersPagedQuery(Page, PageSize);
    }
}
