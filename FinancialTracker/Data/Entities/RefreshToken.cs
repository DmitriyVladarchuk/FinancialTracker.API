namespace FinancialTracker.Data.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; }
    
    public User User { get; set; } = null!;
}