using FCG.Catalog.Application.Common;
using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;

public sealed record GetUsersPagedQuery(
    int Page,
    int PageSize);
