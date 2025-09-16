using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialTracker.Data.Entities;

public class Transaction
{
    public int Id { get; set; }
    public required string Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}