using FinancialTracker.DTOs.TransactionDtos;
using FinancialTracker.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController(
    ITransactionService transactionService,
    ILogger<TransactionsController> logger) : ControllerBase, ITransactionsController
{
    [HttpPost]
    public async Task<ActionResult<TransactionResponseDto>> Add(TransactionCreateDto createDto)
    {
        try
        {
            var transaction = await transactionService.AddAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Ошибка при создании транзакции: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при создании транзакции");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при создании транзакции" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetAll()
    {
        try
        {
            var transactions = await transactionService.GetAllAsync();
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении всех транзакций");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при получении транзакций" });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionResponseDto>> GetById(int id)
    {
        try
        {
            var transaction = await transactionService.GetByIdAsync(id);
            if (transaction != null) return Ok(transaction);
            
            logger.LogWarning("Транзакция не найдена: {Id}", id);
            return NotFound(new { message = $"Транзакция с ID {id} не найдена" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении транзакции по ID: {Id}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при получении транзакции" });
        }
    }

    [HttpPut]
    public async Task<ActionResult<TransactionResponseDto>> Update(TransactionUpdateDto updateDto)
    {
        try
        {
            var transaction = await transactionService.UpdateAsync(updateDto);
            if (transaction != null) return Ok(transaction);
            
            logger.LogWarning("Транзакция не найдена для обновления: {Id}", updateDto.Id);
            return NotFound(new { message = $"Транзакция с ID {updateDto.Id} не найдена" });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Ошибка при обновлении транзакции: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении транзакции: {Id}", updateDto.Id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при обновлении транзакции" });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<TransactionResponseDto>> Delete(int id)
    {
        try
        {
            var transaction = await transactionService.DeleteAsync(id);
            if (transaction != null) return Ok(transaction);
            
            logger.LogWarning("Транзакция не найдена для удаления: {Id}", id);
            return NotFound(new { message = $"Транзакция с ID {id} не найдена" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при удалении транзакции: {Id}", id);
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при удалении транзакции" });
        }
    }
}