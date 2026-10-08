using Microsoft.EntityFrameworkCore;

namespace BackgroundServiceSample.WebMinimalApi.Models
{
    public class RateDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }

        public RateDbContext(DbContextOptions<RateDbContext> optioins) : base(optioins) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("rate_db");
        }
    }
}
