using FinancialTracker.Controllers;
using FinancialTracker.DTOs.CategoryDtos;
using FinancialTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinancialTracker.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly Mock<ILogger<CategoriesController>> _loggerMock;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _loggerMock = new Mock<ILogger<CategoriesController>>();
        _controller = new CategoriesController(_categoryServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Add_ValidCategory_ReturnsCreatedResult()
    {
        var createDto = new CategoryCreateDto { Name = "Продукты" };
        var expectedCategory = new CategoryResponseDto { Id = 1, Name = "Продукты" };
        
        _categoryServiceMock
            .Setup(service => service.AddAsync(createDto))
            .ReturnsAsync(expectedCategory);
        
        var result = await _controller.Add(createDto);
        
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdAtResult.StatusCode);
        Assert.Equal(expectedCategory, createdAtResult.Value);
    }

    [Fact]
    public async Task Add_DuplicateCategory_ReturnsConflict()
    {
        var createDto = new CategoryCreateDto { Name = "Продукты" };
        const string errorMessage = "Категория с именем 'Продукты' уже существует";
        
        _categoryServiceMock
            .Setup(service => service.AddAsync(createDto))
            .ThrowsAsync(new InvalidOperationException(errorMessage));
        
        var result = await _controller.Add(createDto);
        
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithCategories_ReturnsOkResult()
    {
        List<CategoryResponseDto> categories = 
        [
            new() { Id = 1, Name = "Продукты"},
            new() { Id = 2, Name = "Транспорт"},
            new() { Id = 3, Name = "Разное"},
        ];
        
        _categoryServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(categories);
        
        var result = await _controller.GetAll();
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        
        var actualCategories = Assert.IsType<List<CategoryResponseDto>>(okResult.Value);
        Assert.Equal(categories.Count, actualCategories.Count);
        Assert.Equal(categories[0].Name, actualCategories[0].Name);
        Assert.Equal(categories[1].Name, actualCategories[1].Name);
        Assert.Equal(categories[2].Name, actualCategories[2].Name);
    }
    
    [Fact]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyList()
    {
        var emptyList = new List<CategoryResponseDto>();
    
        _categoryServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(emptyList);
        
        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualCategories = Assert.IsType<List<CategoryResponseDto>>(okResult.Value);
        Assert.Empty(actualCategories);
    }

    [Fact]
    public async Task GetAll_ServiceThrowsException_ReturnsServerError()
    {
        const string errorMessage = "Внутренняя ошибка сервера при получении категорий";
    
        _categoryServiceMock
            .Setup(service => service.GetAllAsync())
            .ThrowsAsync(new Exception(errorMessage));

        var result = await _controller.GetAll();

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    
    [Fact]
    public async Task GetById_ExistingCategory_ReturnsOk()
    {
        var categoryId = 1;
        var expectedCategory = new CategoryResponseDto { Id = categoryId, Name = "Продукты" };
    
        _categoryServiceMock
            .Setup(service => service.GetByIdAsync(categoryId))
            .ReturnsAsync(expectedCategory);

        var result = await _controller.GetById(categoryId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualCategory = Assert.IsType<CategoryResponseDto>(okResult.Value);
        Assert.Equal(expectedCategory.Id, actualCategory.Id);
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
    }

    [Fact]
    public async Task GetById_NonExistingCategory_ReturnsNotFound()
    {
        const int categoryId = 999;
    
        _categoryServiceMock
            .Setup(service => service.GetByIdAsync(categoryId))
            .ReturnsAsync((CategoryResponseDto)null!);

        var result = await _controller.GetById(categoryId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task Update_ValidCategory_ReturnsOk()
    {
        var updateDto = new CategoryUpdateDto { Id = 1, Name = "Продукты обновленные" };
        var expectedCategory = new CategoryResponseDto { Id = 1, Name = "Продукты обновленные" };
    
        _categoryServiceMock
            .Setup(service => service.UpdateAsync(updateDto))
            .ReturnsAsync(expectedCategory);

        var result = await _controller.Update(updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualCategory = Assert.IsType<CategoryResponseDto>(okResult.Value);
        Assert.Equal(expectedCategory.Id, actualCategory.Id);
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
    }

    [Fact]
    public async Task Update_DuplicateCategoryName_ReturnsConflict()
    {
        var updateDto = new CategoryUpdateDto { Id = 1, Name = "Дубликат" };
        const string errorMessage = "Попытка обновления категории на дублирующее имя: 1 -> Дубликат";
    
        _categoryServiceMock
            .Setup(service => service.UpdateAsync(updateDto))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        var result = await _controller.Update(updateDto);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ExistingCategory_ReturnsOk()
    {
        const int categoryId = 1;
        var expectedCategory = new CategoryResponseDto { Id = categoryId, Name = "Продукты" };
    
        _categoryServiceMock
            .Setup(service => service.DeleteAsync(categoryId))
            .ReturnsAsync(expectedCategory);

        var result = await _controller.Delete(categoryId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualCategory = Assert.IsType<CategoryResponseDto>(okResult.Value);
        Assert.Equal(expectedCategory.Id, actualCategory.Id);
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
    }

    [Fact]
    public async Task Delete_NonExistingCategory_ReturnsNotFound()
    {
        const int categoryId = 999;
    
        _categoryServiceMock
            .Setup(service => service.DeleteAsync(categoryId))
            .ReturnsAsync((CategoryResponseDto)null!);

        var result = await _controller.Delete(categoryId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}