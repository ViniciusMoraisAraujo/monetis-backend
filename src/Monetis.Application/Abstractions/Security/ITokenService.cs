namespace Monetis.Application.Abstractions.Security;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
}