using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Abstractions.Services;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserAuthService userAuthService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserRequest loginUserRequest, CancellationToken cancellationToken)
    {
        var token = await userAuthService.LoginAsync(loginUserRequest, cancellationToken);
        
        if (token == null)
            return Unauthorized("Invalid credentials");
        
        return Ok(new {Token = token});
    }
}
