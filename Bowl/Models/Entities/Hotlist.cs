using Microsoft.EntityFrameworkCore;

namespace Bowl.Models.Entities
{
    public class Hotlist
    {
        public string Hash { get; }
        public string Content { get; }
        public int Platform { get; }
        public DateTime Date { get; }
        public string Link { get; }
        static private string TableName = "work_hotlist";
        static public void onCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotlist>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(e => e.Hash);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(255);
            });
        }
    }
}
