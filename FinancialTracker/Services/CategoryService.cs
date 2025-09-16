using FinancialTracker.Data.Entities;
using FinancialTracker.DTOs.CategoryDtos;
using FinancialTracker.Interfaces;

namespace FinancialTracker.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        return categories.Select(MapToResponseDto).ToList();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToResponseDto(category);
    }
    
    public async Task<CategoryResponseDto> AddAsync(CategoryCreateDto createDto)
    {
        var existingCategory = await categoryRepository.GetByNameAsync(createDto.Name);
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Категория с именем '{createDto.Name}' уже существует");
        }

        var category = new Category
        {
            Name = createDto.Name
        };

        var createdCategory = await categoryRepository.AddAsync(category);
        return MapToResponseDto(createdCategory);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(CategoryUpdateDto updateDto)
    {
        var existingCategory = await categoryRepository.GetByIdAsync(updateDto.Id);
        if (existingCategory == null)
            return null;

        var duplicateCategory = await categoryRepository.GetByNameAsync(updateDto.Name);
        if (duplicateCategory != null && duplicateCategory.Id != updateDto.Id)
        {
            throw new InvalidOperationException($"Категория с именем '{updateDto.Name}' уже существует");
        }

        existingCategory.Name = updateDto.Name;
        var updatedCategory = await categoryRepository.UpdateAsync(existingCategory);
        return MapToResponseDto(updatedCategory);
    }

    public async Task<CategoryResponseDto?> DeleteAsync(CategoryDeleteDto deleteDto)
    {
        var existingCategory = await categoryRepository.GetByIdAsync(deleteDto.Id);
        if (existingCategory == null)
            return null;

        var deletedCategory = await categoryRepository.DeleteAsync(existingCategory);
        return MapToResponseDto(deletedCategory);
    }

    private static CategoryResponseDto MapToResponseDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}