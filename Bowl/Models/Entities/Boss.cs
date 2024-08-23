using Microsoft.EntityFrameworkCore;

namespace Bowl.Models.Entities
{
    public class Boss
    {
        public string Hash { get; }
        public string Content { get; }
        public string Link { get; }
        public DateTime Date { get; }
        public int Type { get; }
        static private string TableName = "work_boss";
        static public void onCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Boss>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(e => e.Hash);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Link).IsRequired().HasMaxLength(511);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Type).IsRequired();
            });
        }
    }
}
