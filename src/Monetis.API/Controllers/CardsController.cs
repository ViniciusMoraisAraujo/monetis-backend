using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Abstractions.Services;

namespace Monetis.API.Controllers;

[Authorize]
public class CardsController(ICardService cardService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<CardResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var card = await cardService.GetByIdAsync(id, cancellationToken);
        if (card == null)
            return NotFound();

        return Ok(card);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var cards = await cardService.GetAllAsync(cancellationToken);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CardResponse>> Create(
        [FromBody] CreateCardRequest createCardRequest,
        CancellationToken cancellationToken)
    {
        var card = await cardService.CreateAsync(createCardRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = card.Id }, card);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCardRequest updateCardRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await cardService.UpdateAsync(id, updateCardRequest, cancellationToken);
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
        await cardService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
