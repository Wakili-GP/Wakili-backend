using Wakiliy.Domain.Entities;

namespace Wakiliy.Application.Interfaces.Services;
public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(AppUser user, IEnumerable<string> roles);
}
