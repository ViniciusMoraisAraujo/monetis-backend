using Microsoft.AspNetCore.Mvc;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;

namespace Monetis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();
            
        return Ok(category);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
    {
        var category = await _categoryService.CreateAsync(createCategoryDto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            await _categoryService.UpdateAsync(id, updateCategoryDto);
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
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}