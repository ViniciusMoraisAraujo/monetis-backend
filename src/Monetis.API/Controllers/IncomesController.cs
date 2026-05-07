using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[Authorize]
public class IncomesController(IIncomeService incomeService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<IncomeResponse>> GetById([FromRoute] Guid id)
    {
        var income = await incomeService.GetByIdAsync(id);
        if (income == null)
            return NotFound();

        return Ok(income);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncomeResponse>>> GetAll()
    {
        var incomes = await incomeService.GetAllAsync();
        return Ok(incomes);
    }

    [HttpPost]
    public async Task<ActionResult<IncomeResponse>> Create([FromBody] CreateIncomeRequest request)
    {
        try
        {
            var income = await incomeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = income.Id }, income);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateIncomeRequest request)
    {
        try
        {
            await incomeService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            await incomeService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
