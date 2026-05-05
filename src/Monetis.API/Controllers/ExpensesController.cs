using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IExpenseService expenseService) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var expense = await expenseService.GetByIdAsync(id, cancellationToken);
        if (expense == null)
            return NotFound();

        return Ok(expense);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var expenses = await expenseService.GetAllAsync(cancellationToken);
        return Ok(expenses);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var expense = await expenseService.CreateExpenseAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("installments")]
    public async Task<ActionResult<IReadOnlyCollection<ExpenseResponse>>> CreateInstallments(
        [FromBody] CreateInstallmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var expenses = await expenseService.CreateInstallmentAsync(request, cancellationToken);

            return Ok(expenses);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<ExpenseResponse>> Pay(
        [FromRoute] Guid id,
        [FromBody] PayExpenseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var expense = await expenseService.PayExpenseAsync(id, request, cancellationToken);
            return Ok(expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var expense = await expenseService.UpdateExpenseAsync(id, request, cancellationToken);
            return Ok(expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("process-overdue")]
    public async Task<IActionResult> ProcessOverdue(CancellationToken cancellationToken)
    {
        await expenseService.ProcessOverdueExpensesAsync(cancellationToken);
        return NoContent();
    }
}
