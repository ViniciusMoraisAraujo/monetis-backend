using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);
        if (subscription == null)
            return NotFound();
            
        return Ok(subscription);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subscriptions = await _subscriptionService.GetAllAsync();
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionDto createSubscriptionDto)
    {
        var subscription = await _subscriptionService.CreateAsync(createSubscriptionDto);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSubscriptionDto updateSubscriptionDto)
    {
        try
        {
            await _subscriptionService.UpdateAsync(id, updateSubscriptionDto);
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
        await _subscriptionService.DeleteAsync(id);
        return NoContent();
    }
}