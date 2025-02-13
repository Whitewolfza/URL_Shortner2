using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<short_urls> short_urls { get; set; }
    public DbSet<url_access> url_access { get; set; }
}