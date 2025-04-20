using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using web.Server.Data;

namespace web.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240420000000_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("web.Server.Models.NovelGeneration", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("TEXT");

                b.Property<string>("Credentials")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<DateTime?>("CompletedAt")
                    .HasColumnType("TEXT");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("ErrorMessage")
                    .HasColumnType("TEXT");

                b.Property<string>("OutputPath")
                    .HasColumnType("TEXT");

                b.Property<string>("Request")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("NovelGenerations");
            });
        }
    }
} 