using FinancialTracker.DTOs.CategoryDtos;

namespace FinancialTracker.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto?> GetByIdAsync(int id);
    Task<CategoryResponseDto> AddAsync(CategoryCreateDto createDto);
    Task<CategoryResponseDto?> UpdateAsync(CategoryUpdateDto updateDto);
    Task<CategoryResponseDto?> DeleteAsync(int id);
}