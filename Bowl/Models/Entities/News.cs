using Microsoft.EntityFrameworkCore;

namespace Bowl.Models.Entities
{
    public class News
    {
        public string Hash { get; }
        public string Link { get; }
        public int Medium { get; }
        public string Title { get; }
        public DateTime Date { get; }
        public int Status { get; }
        public string? Tags { get; set; }
        public string Keyword { get; }

        static private string TableName = "work_news";
        static public void onCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(e => e.Hash);
                entity.Property(e => e.Link).IsRequired().HasMaxLength(511);
                entity.Property(e => e.Medium).IsRequired();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(511);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Tags).HasMaxLength(127);
                entity.Property(e => e.Keyword).IsRequired().HasMaxLength(63);
            });
        }
    }
}
