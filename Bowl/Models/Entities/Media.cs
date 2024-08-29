using Microsoft.EntityFrameworkCore;

namespace Bowl.Models.Entities
{
    public class Media
    {
        public int Id { get; }
        public string Name { get; }
        public string Domain { get; }

        static private string TableName = "work_media";
        static public void onCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Media>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(63);
                entity.Property(e => e.Domain).IsRequired().HasMaxLength(127);
            });
        }
    }
}
