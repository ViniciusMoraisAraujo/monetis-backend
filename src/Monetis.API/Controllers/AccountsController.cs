using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute]Guid id)
    {
        var accountDto = await accountService.GetByIdAsync(id);
        
        if (accountDto == null)
            return NotFound();
            
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await accountService.GetAllAsync();
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateAccountDto createAccountDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var account = await accountService.CreateAsync(createAccountDto, userId);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateAccountDto updateAccountDto)
    {
        try
        {
            await accountService.UpdateAsync(id, updateAccountDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]Guid id)
    {
        await accountService.DeleteAsync(id);
        return NoContent();
    }
}