using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Queries.Login;

public interface ILoginQueryHandler
{
    Task<ResultData<LoginQueryOutput>> Handle(
    LoginQuery query,
    CancellationToken cancellationToken);
}
