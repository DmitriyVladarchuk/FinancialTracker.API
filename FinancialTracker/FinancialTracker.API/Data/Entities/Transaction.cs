using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialTracker.Data.Entities;

public class Transaction
{
    public int Id { get; set; }
    public required string Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public required decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public required int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}