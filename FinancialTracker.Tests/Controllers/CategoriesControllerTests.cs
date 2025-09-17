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
}