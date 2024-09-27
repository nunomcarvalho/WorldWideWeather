using Microsoft.EntityFrameworkCore;

namespace WorldWideWeather.Business.Model
{
    public class SQLiteDbContext : DbContext
    {
        public SQLiteDbContext(DbContextOptions<SQLiteDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(e => e.Id);
            base.OnModelCreating(builder);
        }
    }
}
