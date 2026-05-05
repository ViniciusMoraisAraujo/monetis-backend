using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransfersController(ITransferService transferService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TransferResponse>> GetById([FromRoute] Guid id)
    {
        var transfer = await transferService.GetByIdAsync(id);
        if (transfer == null)
            return NotFound();

        return Ok(transfer);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransferResponse>>> GetAll()
    {
        var transfers = await transferService.GetAllAsync();
        return Ok(transfers);
    }

    [HttpPost]
    public async Task<ActionResult<TransferResponse>> Create([FromBody] CreateTransferRequest createTransferRequest)
    {
        var transfer = await transferService.CreateAsync(createTransferRequest);
        return CreatedAtAction(nameof(GetById), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTransferRequest updateTransferRequest)
    {
        try
        {
            await transferService.UpdateAsync(id, updateTransferRequest);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await transferService.DeleteAsync(id);
        return NoContent();
    }
}
