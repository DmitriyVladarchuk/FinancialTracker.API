namespace FinancialTracker.Data.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public Guid UserId { get; set; }
    
    public List<Transaction> Transactions { get; set; } = new();
    public User User { get; set; } = null!;
}