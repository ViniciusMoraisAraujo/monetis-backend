using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[Authorize]
public class SubscriptionsController(ISubscriptionService subscriptionService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionResponse>> GetById(Guid id)
    {
        var subscription = await subscriptionService.GetByIdAsync(id);
        if (subscription == null)
            return NotFound();
            
        return Ok(subscription);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> GetAll()
    {
        var subscriptions = await subscriptionService.GetAllAsync();
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> Create(CreateSubscriptionRequest request)
    {
        var subscription = await subscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateSubscriptionRequest request)
    {
        try
        {
            await subscriptionService.UpdateAsync(id, request);
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
        await subscriptionService.DeleteAsync(id);
        return NoContent();
    }
}
