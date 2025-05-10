
using Microsoft.EntityFrameworkCore;
using QuestionService.Entities;
using QuestionService.Enums;

namespace QuestionService.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // For user
            builder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.QuestionId);

                entity.Property(q => q.QuestionId)
                .IsRequired()
                .HasMaxLength(36);

                entity.Property(e => e.Title)
                .HasMaxLength(255);
                entity.HasIndex(q => q.Title);

                entity.Property(e => e.Description)
                .HasColumnType("TEXT");

                entity.Property(e => e.TimeLimit)
                .HasMaxLength(255);

                entity.Property(e => e.MemoryLimit)
                .HasMaxLength(255);

                entity.Property(q => q.Difficulty)
                .HasConversion(
                    v => v.ToString(),     // Convert enum to string when saving
                    v => Enum.Parse<DifficultyEnum>(v) // Convert string back to enum when loading
                );

                entity.Property(e => e.Description)
                .HasMaxLength(255);

                entity.Property(q => q.CreatedBy)
                .IsRequired()
                .HasMaxLength(36);

                entity.Property(q => q.IsDeleted)
                .HasDefaultValue(false);

                entity.HasQueryFilter(q => !q.IsDeleted);

                entity.HasIndex(q => q.CreatedAt);
                entity.HasIndex(q => q.IsDeleted);

            });
        }
    }


}