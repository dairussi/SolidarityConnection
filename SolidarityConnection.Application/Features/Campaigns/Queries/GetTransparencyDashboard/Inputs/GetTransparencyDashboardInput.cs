namespace SolidarityConnection.Application.Features.Campaigns.Queries.GetTransparencyDashboard.Inputs;

public sealed record GetTransparencyDashboardInput(int Page = 1, int PageSize = 20)
{
    public GetTransparencyDashboardQuery MapToQuery() => new(Page, PageSize);
}