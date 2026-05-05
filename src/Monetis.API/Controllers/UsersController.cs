using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService, IUserAuthService userAuthService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("User not found.");
            
        return Ok(user);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest createUserRequest)
    {
        var user = await userService.CreateAsync(createUserRequest);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginUserRequest loginUserRequest)
    {
        var user = await userAuthService.LoginAsync(loginUserRequest);
        if (user == null)
            return NotFound("User not found.");
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest updateUserRequest)
    {
        try
        {
            await userService.UpdateAsync(id, updateUserRequest);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await userService.DeleteAsync(id);
        try
        {
            await userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found.");
        }
    }
}
