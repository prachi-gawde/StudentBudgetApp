using Microsoft.EntityFrameworkCore;
using StudentBudgetApp.Models;

namespace StudentBudgetApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BudgetEntry> BudgetEntries => Set<BudgetEntry>();
    }
}