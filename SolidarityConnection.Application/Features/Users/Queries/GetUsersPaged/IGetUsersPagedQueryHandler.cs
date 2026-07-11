using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;

public interface IGetUsersPagedQueryHandler
{
    Task<ResultData<PagedResult<UserOutput>>> Handle(
        GetUsersPagedQuery query,
        CancellationToken cancellationToken);
}
