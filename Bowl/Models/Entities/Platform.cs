using Microsoft.EntityFrameworkCore;

namespace Bowl.Models.Entities
{
    public class Platform
    {
        public int Id { get; }
        public string Name { get; set; }

        static private string TableName = "work_platform";
        static public void onCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>(entity =>
            {
                entity.ToTable(TableName);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(63);
            });
        }
    }
}
