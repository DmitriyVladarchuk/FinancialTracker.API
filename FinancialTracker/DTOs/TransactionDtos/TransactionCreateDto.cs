namespace FinancialTracker.DTOs.TransactionDtos;

public class TransactionCreateDto
{
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
}