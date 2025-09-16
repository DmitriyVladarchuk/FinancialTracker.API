using FinancialTracker.DTOs.TransactionDtos;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTracker.Interfaces;

public interface ITransactionsController
{
    Task<ActionResult<TransactionResponseDto>> Add(TransactionCreateDto createDto);
    Task<ActionResult<List<TransactionResponseDto>>> GetAll();
    Task<ActionResult<TransactionResponseDto>> GetById(int id);
    Task<ActionResult<TransactionResponseDto>> Update(TransactionUpdateDto updateDto);
    Task<ActionResult<TransactionResponseDto>> Delete(int id);
}