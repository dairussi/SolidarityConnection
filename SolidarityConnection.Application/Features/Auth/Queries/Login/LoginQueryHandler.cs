using SolidarityConnection.Application.Common;
using SolidarityConnection.Application.Common.Interfaces;
using SolidarityConnection.Application.Features.Auth.Queries.Login.Outputs;

namespace SolidarityConnection.Application.Features.Auth.Queries.Login;
public class LoginQueryHandler : ILoginQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginQueryHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResultData<LoginQueryOutput>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user == null || !user.IsActive)
            return ResultData<LoginQueryOutput>.Error("Usuário ou senha inválidos.");

        if (!_passwordHasher.Verify(query.Password, user.PasswordHash))
            return ResultData<LoginQueryOutput>.Error("Usuário ou senha inválidos.");

        var token = _tokenService.GenerateToken(user.PublicId, user.Role.ToString());

        return ResultData<LoginQueryOutput>.Success(new LoginQueryOutput
        {
            Token = token,
            Name = user.Name,
            Role = user.Role.ToString()
        });
    }
}
