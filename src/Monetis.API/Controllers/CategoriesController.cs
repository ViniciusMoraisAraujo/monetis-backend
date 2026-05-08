using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Abstractions.Services;

namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return NotFound();
            
        return Ok(category);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(
        [FromBody] CreateCategoryRequest createCategoryRequest,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateAsync(createCategoryRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest updateCategoryRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await categoryService.UpdateAsync(id, updateCategoryRequest, cancellationToken);
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
        await categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
