namespace SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;

public sealed record GetUsersPagedQuery(
    int Page,
    int PageSize);
