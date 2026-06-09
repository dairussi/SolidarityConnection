using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Queries.Login;
public class LoginQueryHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher) : ILoginQueryHandler
{
    public async Task<ResultData<LoginQueryOutput>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user == null || !user.IsActive)
            return ResultData<LoginQueryOutput>.Error("Usuário ou senha inválidos.");

        if (!passwordHasher.Verify(query.Password, user.PasswordHash))
            return ResultData<LoginQueryOutput>.Error("Usuário ou senha inválidos.");

        var token = tokenService.GenerateToken(user.Id, user.Role.ToString());

        return ResultData<LoginQueryOutput>.Success(new LoginQueryOutput
        {
            Token = token,
            Name = user.Name,
            Role = user.Role.ToString()
        });
    }
}
