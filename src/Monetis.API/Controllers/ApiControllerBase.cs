using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public abstract class ApiControllerBase : ControllerBase
{
    private Guid? _userId;
    protected Guid UserId => _userId ??= GetUserId();

    private bool TryGetUserId(out Guid userId)
    {
        if (_userId.HasValue)
        {
            userId = _userId.Value;
            return true;
        }
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (Guid.TryParse(userIdClaim, out userId))
        {
            _userId = userId;
            return true;
        }

        return false;
    }

    private Guid GetUserId() =>
        TryGetUserId(out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User ID claim is missing or invalid.");

}
