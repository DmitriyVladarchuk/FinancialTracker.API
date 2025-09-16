using FinancialTracker.DTOs.CategoryDtos;
using FinancialTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    ICategoryService categoryService,
    ILogger<CategoriesController> logger) : ControllerBase, ICategoriesController
{
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Add(CategoryCreateDto createDto)
    {
        try
        {
            var category = await categoryService.AddAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Попытка создания дубликата категории: {Name}", createDto.Name);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при создании категории: {Name}", createDto.Name);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при создании категории" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetAll()
    {
        try
        {
            var categories = await categoryService.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении всех категорий");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при получении категорий" });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        try
        {
            var category = await categoryService.GetByIdAsync(id);
            if (category != null) return Ok(category);
            
            logger.LogWarning("Категория не найдена: {Id}", id);
            return NotFound(new { message = $"Категория с ID {id} не найдена" });

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении категории по ID: {Id}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при получении категории" });
        }
    }

    [HttpPut]
    public async Task<ActionResult<CategoryResponseDto>> Update(CategoryUpdateDto updateDto)
    {
        try
        {
            var category = await categoryService.UpdateAsync(updateDto);
            if (category != null) return Ok(category);
            
            logger.LogWarning("Категория не найдена для обновления: {Id}", updateDto.Id);
            return NotFound(new { message = $"Категория с ID {updateDto.Id} не найдена" });

        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Попытка обновления категории на дублирующее имя: {Id} -> {Name}", 
                updateDto.Id, updateDto.Name);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении категории: {Id}", updateDto.Id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при обновлении категории" });
        }
    }

    [HttpDelete]
    public async Task<ActionResult<CategoryResponseDto>> Delete(CategoryDeleteDto deleteDto)
    {
        try
        {
            var category = await categoryService.DeleteAsync(deleteDto);
            if (category != null) return Ok(category);
            
            logger.LogWarning("Категория не найдена для удаления: {Id}", deleteDto.Id);
            return NotFound(new { message = $"Категория с ID {deleteDto.Id} не найдена" });

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при удалении категории: {Id}", deleteDto.Id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при удалении категории" });
        }
    }
}