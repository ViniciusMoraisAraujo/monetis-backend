using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null)
            return NotFound();
            
        return Ok(transaction);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto createTransactionDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var transaction = await _transactionService.CreateAsync(createTransactionDto, userId);
        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTransactionDto updateTransactionDto)
    {
        try
        {
            await _transactionService.UpdateAsync(id, updateTransactionDto);
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
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }
}