using InventaiNovel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Server.Models
{
    public class NovelGeneration
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
        public NovelCreationRequest Request { get; set; } = null!;

        [Required]
        public ApiCredentials Credentials { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Failed

        public string? ErrorMessage { get; set; }

        public string? OutputPath { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }
    }
} 