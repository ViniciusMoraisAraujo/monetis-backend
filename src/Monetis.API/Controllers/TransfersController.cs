using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Abstractions.Services;

namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransfersController(ITransferService transferService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TransferResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var transfer = await transferService.GetByIdAsync(id, cancellationToken);
        if (transfer == null)
            return NotFound();

        return Ok(transfer);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransferResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var transfers = await transferService.GetAllAsync(cancellationToken);
        return Ok(transfers);
    }

    [HttpPost]
    public async Task<ActionResult<TransferResponse>> Create(
        [FromBody] CreateTransferRequest createTransferRequest,
        CancellationToken cancellationToken)
    {
        var transfer = await transferService.CreateAsync(createTransferRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateTransferRequest updateTransferRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await transferService.UpdateAsync(id, updateTransferRequest, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await transferService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
