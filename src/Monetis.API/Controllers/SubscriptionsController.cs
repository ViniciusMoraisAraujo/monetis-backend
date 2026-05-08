using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Abstractions.Services;

namespace Monetis.API.Controllers;

[Authorize]
public class SubscriptionsController(ISubscriptionService subscriptionService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionService.GetByIdAsync(id, cancellationToken);
        if (subscription == null)
            return NotFound();
            
        return Ok(subscription);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var subscriptions = await subscriptionService.GetAllAsync(cancellationToken);
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> Create(
        CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute]Guid id,
        [FromBody] UpdateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await subscriptionService.UpdateAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await subscriptionService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
