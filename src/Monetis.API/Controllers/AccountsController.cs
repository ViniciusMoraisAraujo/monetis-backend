using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService accountService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponse>> GetById([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        var accountDto = await accountService.GetByIdAsync(id, cancellationToken);
        
        if (accountDto == null)
            return NotFound();
            
        return Ok(accountDto);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await accountService.GetAllAsync(cancellationToken);
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(
        [FromBody]CreateAccountRequest createAccountRequest,
        CancellationToken cancellationToken)
    {
        var account = await accountService.CreateAsync(createAccountRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute]Guid id,
        [FromBody]UpdateAccountRequest updateAccountRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await accountService.UpdateAsync(id, updateAccountRequest, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        await accountService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
