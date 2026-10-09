using Microsoft.EntityFrameworkCore;

namespace BackgroundServiceSample.WorkerServiceSample.Models
{
    public class RateWorkerServiceDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }

        public RateWorkerServiceDbContext(
            DbContextOptions<RateWorkerServiceDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("rate_db");
        }
    }
}
