namespace FinancialTracker.DTOs.TransactionDtos;

public class TransactionResponseDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public required string CategoryName { get; set; }
}