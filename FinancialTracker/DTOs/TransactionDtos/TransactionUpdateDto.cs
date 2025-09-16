namespace FinancialTracker.DTOs.TransactionDtos;

public class TransactionUpdateDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
}