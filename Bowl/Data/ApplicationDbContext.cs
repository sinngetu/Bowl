using Microsoft.EntityFrameworkCore;
using Bowl.Models.Entities;

namespace Bowl.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Media> Media { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Keyword> Keyword { get; set; }
        public DbSet<Platform> Platform { get; set; }
        public DbSet<Hotlist> Hotlist { get; set; }
        public DbSet<Boss> Boss { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            Models.Entities.Media.onCreating(builder);
            Models.Entities.News.onCreating(builder);
            Models.Entities.Keyword.onCreating(builder);
            Models.Entities.Platform.onCreating(builder);
            Models.Entities.Hotlist.onCreating(builder);
            Models.Entities.Boss.onCreating(builder);
        }
    }
}
