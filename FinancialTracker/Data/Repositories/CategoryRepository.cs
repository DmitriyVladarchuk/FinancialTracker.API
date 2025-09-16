using FinancialTracker.Data.Entities;
using FinancialTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Data.Repositories;

public class CategoryRepository(FinancialDbContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync()
    {
        return await context.Categories.ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories.FindAsync(id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<Category> AddAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> DeleteAsync(Category category)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return category;
    }
}