using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Queries.Login;
public class LoginQueryHandler : ILoginQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginQueryHandler(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ResultData<LoginQueryOutput>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user == null || !user.IsActive)
            return ResultData<LoginQueryOutput>.Error("Usuário ou senha inválidos.");

        if (!BCrypt.Net.BCrypt.Verify(query.Password, user.PasswordHash))
            return ResultData<LoginQueryOutput>.Error("Usuário ou senha inválidos.");

        var token = _tokenService.GenerateToken(user.Id, user.Role);

        return ResultData<LoginQueryOutput>.Success(new LoginQueryOutput
        {
            Token = token,
            Name = user.Name,
            Role = user.Role
        });
    }
}
