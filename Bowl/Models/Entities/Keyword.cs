using Microsoft.EntityFrameworkCore;

namespace Bowl.Models.Entities
{
    public enum KeywordType
    {
        Hotlist = 0,
        Tag = 1,
        Overseas = 2,
        Search = 3,
        WandaSearch = 4,
        Inland = 5,
    }

    public class Keyword
    {
        public int Id { get; }
        public string Word { get; set; }
        public int Type { get; set; }
        public string? Extend { get; set; }
        static private string TableName = "work_keyword";
        static public void onCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Word).IsRequired().HasMaxLength(63);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Extend).HasMaxLength(1023);
            });
        }
    }
}
