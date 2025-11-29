using FinancialTracker.DTOs.TransactionDtos;

namespace FinancialTracker.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetAllAsync();
    Task<TransactionResponseDto?> GetByIdAsync(int id);
    Task<TransactionResponseDto> AddAsync(TransactionCreateDto createDto);
    Task<TransactionResponseDto?> UpdateAsync(TransactionUpdateDto updateDto);
    Task<TransactionResponseDto?> DeleteAsync(int id);
}