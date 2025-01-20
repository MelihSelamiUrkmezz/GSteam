using GameService.DTOs;
using GameService.Repositories.ForCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCategories()
    {
        var response = await _categoryRepository.GetAllCategories();
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult> CreateCategory(CategoryDTO model)
    {
        var response = await _categoryRepository.CreateCategory(model);
        return Ok(response);
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult> RemoveCategory([FromRoute]Guid categoryId)
    {
        var response = await _categoryRepository.RemoveCategory(categoryId);
        return Ok(response);
    }

    [HttpPut("{categoryId}")]
    public async Task<ActionResult> UpdateCategory(CategoryDTO model,Guid categoryId)
    {
        var response = await _categoryRepository.UpdateCategory(model,categoryId);
        return Ok(response);
    }
}