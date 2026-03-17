using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null)
            return NotFound();
            
        return Ok(account);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _accountService.GetAllAsync();
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountDto createAccountDto)
    {
        var account = await _accountService.CreateAsync(createAccountDto);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAccountDto updateAccountDto)
    {
        try
        {
            await _accountService.UpdateAsync(id, updateAccountDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _accountService.DeleteAsync(id);
        return NoContent();
    }
}