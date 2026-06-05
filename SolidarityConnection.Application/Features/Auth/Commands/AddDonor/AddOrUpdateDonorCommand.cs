using SolidarityConnection.Domain.Common.Enums;
using SolidarityConnection.Domain.Users.ValueObjects;
using System.Data;
using System.Net.NetworkInformation;

namespace SolidarityConnection.Application.Features.Auth.Commands.DonorRegistration;
public class AddOrUpdateDonorCommand
{
    private AddOrUpdateDonorCommand(Guid? publicId, FullName name, EmailAddress email, CpfValidator cpf, RawPassword password, EUserRole role)
    {
        PublicId = publicId;
        Name = name;
        Email = email;
        Cpf = cpf;
        Password = password;
        Role = role;
    }

    public Guid? PublicId { get; }
    public FullName Name { get; }
    public EmailAddress Email { get; }
    public CpfValidator Cpf { get; }
    public RawPassword Password { get; }
    public EUserRole Role { get; set; }

    public static AddOrUpdateDonorCommand Create(Guid? publicId, FullName name, EmailAddress email, CpfValidator cpf, RawPassword password, EUserRole role)
    {
        return new AddOrUpdateDonorCommand(publicId, name, email, cpf,password, role);
    }
}
