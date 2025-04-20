using Microsoft.EntityFrameworkCore;
using web.Server.Models;
using InventaiNovel;
using System.Text.Json;

namespace web.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NovelGeneration> NovelGenerations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NovelGeneration>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Store complex types as JSON
                entity.Property(e => e.Request)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<NovelCreationRequest>(v, (JsonSerializerOptions)null)
                    );

                entity.Property(e => e.Credentials)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<ApiCredentials>(v, (JsonSerializerOptions)null)
                    );
            });
        }
    }
} 