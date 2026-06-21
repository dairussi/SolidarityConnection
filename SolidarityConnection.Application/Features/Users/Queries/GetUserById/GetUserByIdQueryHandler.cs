using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Users.Mappers;
using SolidarityConnection.Application.Features.Users.Outputs;

namespace SolidarityConnection.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository) : IGetUserByIdQueryHandler
{
    public async Task<ResultData<UserOutput>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(query.PublicId, cancellationToken);

        if (user is null)
        {
            return ResultData<UserOutput>.Error("Usuário não encontrado.");
        }

        return ResultData<UserOutput>.Success(user.ToOutput());
    }
}
