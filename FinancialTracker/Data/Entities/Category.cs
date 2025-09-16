namespace FinancialTracker.Data.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public List<Transaction> Transactions { get; set; } = new();
}