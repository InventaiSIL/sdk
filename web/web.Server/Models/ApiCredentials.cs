using System.ComponentModel.DataAnnotations;

namespace web.Server.Models
{
    public class ApiCredentials
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// OpenAI API key for text generation
        /// </summary>
        [Required]
        public string OpenAiApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Segmind API key for image generation
        /// </summary>
        [Required]
        public string SegmindApiKey { get; set; } = string.Empty;
    }
} 