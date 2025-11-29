using FinancialTracker.Controllers;
using FinancialTracker.DTOs.TransactionDtos;
using FinancialTracker.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinancialTracker.Tests.Controllers;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionService> _transactionServiceMock;
    private readonly Mock<ILogger<TransactionsController>> _loggerMock;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        _transactionServiceMock = new Mock<ITransactionService>();
        _loggerMock = new Mock<ILogger<TransactionsController>>();
        _controller = new TransactionsController(_transactionServiceMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task Add_ValidTransaction_ReturnsCreatedResult()
    {
        var createDto = new TransactionCreateDto 
        { 
            Amount = 1000, 
            Description = "Покупка продуктов",
            CategoryId = 1,
        };
    
        var expectedTransaction = new TransactionResponseDto
        {
            Id = 1,
            Amount = 1000,
            Description = "Покупка продуктов",
            Date = DateTime.Now,
            CategoryName = "Продукты"
        };
    
        _transactionServiceMock
            .Setup(service => service.AddAsync(createDto))
            .ReturnsAsync(expectedTransaction);

        var result = await _controller.Add(createDto);

        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdAtResult.StatusCode);
        Assert.Equal(expectedTransaction, createdAtResult.Value);
    }

    [Fact]
    public async Task Add_ServiceThrowsException_ReturnsServerError()
    {
        var createDto = new TransactionCreateDto 
        { 
            Amount = 1000, 
            Description = "Покупка продуктов",
            CategoryId = 1
        };
    
        _transactionServiceMock
            .Setup(service => service.AddAsync(createDto))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.Add(createDto);

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    
    [Fact]
    public async Task GetAll_WithTransactions_ReturnsOk()
    {
        var expectedTransactions = new List<TransactionResponseDto>
        {
            new()
            {
                Id = 1,
                Amount = 1000,
                Description = "Продукты",
                CategoryName = "Еда",
                CategoryId = 1,
            },
            new()
            {
                Id = 2,
                Amount = 500,
                Description = "Транспорт",
                CategoryName = "Транспорт",
                CategoryId = 2,
            }
        };
        
        _transactionServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(expectedTransactions);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        
        var actualTransactions = Assert.IsType<List<TransactionResponseDto>>(okResult.Value);
        Assert.Equal(expectedTransactions.Count, actualTransactions.Count);
        Assert.Equal(expectedTransactions[0].Id, actualTransactions[0].Id);
        Assert.Equal(expectedTransactions[0].Amount, actualTransactions[0].Amount);
        Assert.Equal(expectedTransactions[0].Description, actualTransactions[0].Description);
        Assert.Equal(expectedTransactions[0].CategoryName, actualTransactions[0].CategoryName);
        Assert.Equal(expectedTransactions[0].Date, actualTransactions[0].Date);
        Assert.Equal(expectedTransactions[0].CategoryId, actualTransactions[0].CategoryId);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyList()
    {
        var emptyList = new List<TransactionResponseDto>();
        
        _transactionServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(emptyList);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetAll_ServiceThrowsException_ReturnsInternalServerError()
    {
        _transactionServiceMock
            .Setup(service => service.GetAllAsync())
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetAll();

        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
    
    [Fact]
    public async Task GetById_ExistingTransaction_ReturnsOkWithTransaction()
    {
        const int transactionId = 1;
        var expectedTransaction = new TransactionResponseDto 
        { 
            Id = transactionId, 
            Amount = 1000, 
            Description = "Продукты", 
            CategoryName = "Еда",
            CategoryId = 1,
        };
    
        _transactionServiceMock
            .Setup(service => service.GetByIdAsync(transactionId))
            .ReturnsAsync(expectedTransaction);

        var result = await _controller.GetById(transactionId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualTransaction = Assert.IsType<TransactionResponseDto>(okResult.Value);
        Assert.Equal(expectedTransaction.Id, actualTransaction.Id);
        Assert.Equal(expectedTransaction.Amount, actualTransaction.Amount);
        Assert.Equal(expectedTransaction.Description, actualTransaction.Description);
        Assert.Equal(expectedTransaction.CategoryName, actualTransaction.CategoryName);
        Assert.Equal(expectedTransaction.Date, actualTransaction.Date);
        Assert.Equal(expectedTransaction.CategoryId, actualTransaction.CategoryId);
    }

    [Fact]
    public async Task GetById_NonExistingTransaction_ReturnsNotFound()
    {
        const int transactionId = 999;
    
        _transactionServiceMock
            .Setup(service => service.GetByIdAsync(transactionId))
            .ReturnsAsync((TransactionResponseDto)null!);

        var result = await _controller.GetById(transactionId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task Update_ExistingTransaction_ReturnsOk()
    {
        var updateDto = new TransactionUpdateDto 
        { 
            Id = 1, 
            Amount = 1500, 
            Description = "Продукты обновленные",
            CategoryId = 2
        };
    
        var expectedTransaction = new TransactionResponseDto 
        { 
            Id = 1, 
            Amount = 1500, 
            Description = "Продукты обновленные", 
            CategoryName = "Супермаркет",
            CategoryId = 2,
            Date = DateTime.Now
        };
    
        _transactionServiceMock
            .Setup(service => service.UpdateAsync(updateDto))
            .ReturnsAsync(expectedTransaction);
        
        var result = await _controller.Update(updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualTransaction = Assert.IsType<TransactionResponseDto>(okResult.Value);
        Assert.Equal(expectedTransaction.Id, actualTransaction.Id);
        Assert.Equal(expectedTransaction.Amount, actualTransaction.Amount);
        Assert.Equal(expectedTransaction.Description, actualTransaction.Description);
        Assert.Equal(expectedTransaction.CategoryName, actualTransaction.CategoryName);
        Assert.Equal(expectedTransaction.Date, actualTransaction.Date);
        Assert.Equal(expectedTransaction.CategoryId, actualTransaction.CategoryId);
    }
    
    [Fact]
    public async Task Update_NonExistingTransaction_ReturnsNotFound()
    {
        var updateDto = new TransactionUpdateDto 
        { 
            Id = 999, 
            Amount = 1500, 
            Description = "Несуществующая транзакция",
            CategoryId = 1
        };
    
        _transactionServiceMock
            .Setup(service => service.UpdateAsync(updateDto))
            .ReturnsAsync((TransactionResponseDto)null!);

        var result = await _controller.Update(updateDto);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task Delete_ExistingTransaction_ReturnsOkWithDeletedTransaction()
    {
        const int transactionId = 1;
        var expectedTransaction = new TransactionResponseDto 
        { 
            Id = transactionId, 
            Amount = 1000, 
            Description = "Продукты", 
            CategoryName = "Еда",
            CategoryId = 1,
        };
    
        _transactionServiceMock
            .Setup(service => service.DeleteAsync(transactionId))
            .ReturnsAsync(expectedTransaction);

        var result = await _controller.Delete(transactionId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
    
        var actualTransaction = Assert.IsType<TransactionResponseDto>(okResult.Value);
        Assert.Equal(expectedTransaction.Id, actualTransaction.Id);
        Assert.Equal(expectedTransaction.Amount, actualTransaction.Amount);
        Assert.Equal(expectedTransaction.Description, actualTransaction.Description);
        Assert.Equal(expectedTransaction.CategoryName, actualTransaction.CategoryName);
        Assert.Equal(expectedTransaction.Date, actualTransaction.Date);
        Assert.Equal(expectedTransaction.CategoryId, actualTransaction.CategoryId);
    }

    [Fact]
    public async Task Delete_NonExistingTransaction_ReturnsNotFound()
    {
        const int transactionId = 999;
    
        _transactionServiceMock
            .Setup(service => service.DeleteAsync(transactionId))
            .ReturnsAsync((TransactionResponseDto)null!);

        var result = await _controller.Delete(transactionId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}