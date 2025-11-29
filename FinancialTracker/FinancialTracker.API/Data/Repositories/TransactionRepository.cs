using FinancialTracker.Data.Entities;
using FinancialTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Data.Repositories;

public class TransactionRepository(FinancialDbContext context) : ITransactionRepository
{
    public async Task<List<Transaction>> GetAllAsync()
    {
        return await context.Transactions
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> DeleteAsync(Transaction transaction)
    {
        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }
}