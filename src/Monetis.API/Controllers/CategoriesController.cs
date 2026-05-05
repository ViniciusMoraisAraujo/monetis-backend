using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;

namespace Monetis.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetById([FromRoute] Guid id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();
            
        return Ok(category);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories = await categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest createCategoryRequest)
    {
        var category = await categoryService.CreateAsync(createCategoryRequest);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryRequest updateCategoryRequest)
    {
        try
        {
            await categoryService.UpdateAsync(id, updateCategoryRequest);
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
        await categoryService.DeleteAsync(id);
        return NoContent();
    }
}
