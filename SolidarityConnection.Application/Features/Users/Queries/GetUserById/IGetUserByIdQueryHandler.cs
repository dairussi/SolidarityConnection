using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Queries.GetUserById;

public interface IGetUserByIdQueryHandler
{
    Task<ResultData<UserOutput>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken);
}
