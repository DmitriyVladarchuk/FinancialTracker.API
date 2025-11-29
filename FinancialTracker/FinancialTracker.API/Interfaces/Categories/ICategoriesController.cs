using FinancialTracker.DTOs.CategoryDtos;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Interfaces;

public interface ICategoriesController
{
    Task<ActionResult<CategoryResponseDto>> Add(CategoryCreateDto createDto);
    Task<ActionResult<List<CategoryResponseDto>>> GetAll();
    Task<ActionResult<CategoryResponseDto>> GetById(int id);
    Task<ActionResult<CategoryResponseDto>> Update(CategoryUpdateDto updateDto);
    Task<ActionResult<CategoryResponseDto>> Delete(int id);
}