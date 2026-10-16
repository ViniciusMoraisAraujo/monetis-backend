using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserAuthService userAuthService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody]LoginUserRequest loginUserRequest, CancellationToken cancellationToken)
    {
        var token = await userAuthService.LoginAsync(loginUserRequest, cancellationToken);

        if (token == string.Empty)
            return Unauthorized("Invalid credentials");

        return Ok(new { Token = token });
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest registerUserRequest, CancellationToken cancellationToken)
    {
        await userAuthService.RegisterAsync(registerUserRequest, cancellationToken);

        return Ok();
    }
}
