using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Users.Mappers;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Queries.GetUsersPaged;

public sealed class GetUsersPagedQueryHandler(
    IUserRepository userRepository) : IGetUsersPagedQueryHandler
{

    public async Task<ResultData<PagedResult<UserOutput>>> Handle(
        GetUsersPagedQuery query,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await userRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            cancellationToken);

        var pagedResult = new PagedResult<UserOutput>
        {
            Items = items.ToOutput().ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return ResultData<PagedResult<UserOutput>>.Success(pagedResult);
    }
}
