using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[Authorize]
public class CardsController(ICardService cardService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<CardResponse>> GetById([FromRoute] Guid id)
    {
        var card = await cardService.GetByIdAsync(id);
        if (card == null)
            return NotFound();

        return Ok(card);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardResponse>>> GetAll()
    {
        var cards = await cardService.GetAllAsync();
        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CardResponse>> Create([FromBody] CreateCardRequest createCardRequest)
    {
        var card = await cardService.CreateAsync(createCardRequest);
        return CreatedAtAction(nameof(GetById), new { id = card.Id }, card);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCardRequest updateCardRequest)
    {
        try
        {
            await cardService.UpdateAsync(id, updateCardRequest);
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
        await cardService.DeleteAsync(id);
        return NoContent();
    }
}
