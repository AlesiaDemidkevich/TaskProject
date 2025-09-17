using C_Part1;
using Microsoft.EntityFrameworkCore;

namespace TaskServiceApi.Db
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Description).HasMaxLength(100);
                entity.Property(t => t.IsCompleted).HasDefaultValue(false);

                entity.Property(t => t.DueDate)
                    .HasConversion(
                        v => v.ToDateTime(TimeOnly.MinValue),
                        v => DateOnly.FromDateTime(v)
                    )
                    .HasColumnType("date");
            });
        }
    }
}
