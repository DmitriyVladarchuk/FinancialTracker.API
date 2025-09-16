using FinancialTracker.Data.Entities;
using FinancialTracker.DTOs.TransactionDtos;
using FinancialTracker.Interfaces;

namespace FinancialTracker.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository) : ITransactionService
{
    public async Task<List<TransactionResponseDto>> GetAllAsync()
    {
        var transactions = await transactionRepository.GetAllAsync();
        return transactions.Select(MapToResponseDto).ToList();
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(int id)
    {
        var transaction = await transactionRepository.GetByIdAsync(id);
        return transaction == null ? null : MapToResponseDto(transaction);
    }

    public async Task<TransactionResponseDto> AddAsync(TransactionCreateDto createDto)
    {
        var category = await categoryRepository.GetByIdAsync(createDto.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException($"Категория с ID {createDto.CategoryId} не найдена");
        }

        var transaction = new Transaction
        {
            Description = createDto.Description,
            Amount = createDto.Amount,
            //Date = createDto.Date,
            CategoryId = createDto.CategoryId
        };

        var createdTransaction = await transactionRepository.AddAsync(transaction);
        return MapToResponseDto(createdTransaction);
    }

    public async Task<TransactionResponseDto?> UpdateAsync(TransactionUpdateDto updateDto)
    {
        var existingTransaction = await transactionRepository.GetByIdAsync(updateDto.Id);
        if (existingTransaction == null)
            return null;
        
        var category = await categoryRepository.GetByIdAsync(updateDto.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException($"Категория с ID {updateDto.CategoryId} не найдена");
        }

        existingTransaction.Description = updateDto.Description;
        existingTransaction.Amount = updateDto.Amount;
        existingTransaction.Date = updateDto.Date;
        existingTransaction.CategoryId = updateDto.CategoryId;

        var updatedTransaction = await transactionRepository.UpdateAsync(existingTransaction);
        return MapToResponseDto(updatedTransaction);
    }

    public async Task<TransactionResponseDto?> DeleteAsync(int id)
    {
        var existingTransaction = await transactionRepository.GetByIdAsync(id);
        if (existingTransaction == null)
            return null;

        var deletedTransaction = await transactionRepository.DeleteAsync(existingTransaction);
        return MapToResponseDto(deletedTransaction);
    }

    private static TransactionResponseDto MapToResponseDto(Transaction transaction)
    {
        return new TransactionResponseDto
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Amount = transaction.Amount,
            Date = transaction.Date,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category.Name
        };
    }
}