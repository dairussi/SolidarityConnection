using SolidarityConnection.Application.Features.Users.Outputs;
using SolidarityConnection.Domain.User.Models;

namespace SolidarityConnection.Application.Features.Users.Mappers;
public static class UserOutputMapper
{
    public static UserOutput ToOutput(this User user)
    {
        return new UserOutput(
            user.PublicId,
            user.Name,
            user.Email.Value,
            user.Cpf.Value,
            user.IsActive,
            user.Role.ToString()
        );
    }

    public static List<UserOutput> ToOutput(this IEnumerable<User> users)
    {
        return users.Select(ToOutput).ToList();
    }
}
