using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventaiNovel;
using Inventai.TextAgents;
using Inventai.ImageAgents;
using web.Server.Data;
using web.Server.Models;

namespace web.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class NovelController : ControllerBase
{
    private readonly ILogger<NovelController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public NovelController(ILogger<NovelController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost(Name = "CreateNovel")]
    public async Task<IActionResult> CreateNovel(
        [FromBody] NovelCreationRequest request,
        [FromHeader(Name = "OpenAiApiKey")] string openAiApiKey,
        [FromHeader(Name = "SegmindApiKey")] string segmindApiKey)
    {
        try
        {
            if (string.IsNullOrEmpty(openAiApiKey) || string.IsNullOrEmpty(segmindApiKey))
            {
                return BadRequest(new { error = "API credentials are required" });
            }

            var credentials = new ApiCredentials
            {
                OpenAiApiKey = openAiApiKey,
                SegmindApiKey = segmindApiKey
            };

            var generation = new NovelGeneration
            {
                Id = Guid.NewGuid(),
                Request = request,
                Credentials = credentials,
                Status = "Pending"
            };

            _dbContext.NovelGenerations.Add(generation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { 
                message = "Novel generation started", 
                generationId = generation.Id,
                status = generation.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting novel generation");
            return StatusCode(500, new { error = "Failed to start novel generation", details = ex.Message });
        }
    }

    [HttpGet("{generationId}/status")]
    public async Task<IActionResult> GetGenerationStatus(string generationId)
    {
        try
        {
            if (!Guid.TryParse(generationId, out var id))
            {
                return BadRequest(new { error = "Invalid generation ID format" });
            }

            var generation = await _dbContext.NovelGenerations
                .FirstOrDefaultAsync(g => g.Id == id);

            if (generation == null)
            {
                return NotFound(new { error = "Generation not found" });
            }

            return Ok(new
            {
                status = generation.Status,
                progress = 50,
                error = generation.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking generation status");
            return StatusCode(500, new { error = "Failed to check generation status" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNovel(Guid id)
    {
        try
        {
            var generation = await _dbContext.NovelGenerations
                .FirstOrDefaultAsync(g => g.Id == id);

            if (generation == null)
            {
                return NotFound(new { error = "Novel not found" });
            }

            return Ok(new
            {
                id = generation.Id,
                status = generation.Status,
                createdAt = generation.CreatedAt,
                completedAt = generation.CompletedAt,
                errorMessage = generation.ErrorMessage,
                outputPath = generation.OutputPath
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting novel");
            return StatusCode(500, new { error = "Failed to get novel" });
        }
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadNovel(Guid id)
    {
        try
        {
            var generation = await _dbContext.NovelGenerations
                .FirstOrDefaultAsync(g => g.Id == id);

            if (generation == null)
            {
                return NotFound(new { error = "Novel not found" });
            }

            if (generation.Status != "Completed")
            {
                return BadRequest(new { error = "Novel is not ready for download" });
            }

            if (string.IsNullOrEmpty(generation.OutputPath))
            {
                return NotFound(new { error = "Novel output not found" });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), generation.OutputPath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { error = "Novel file not found" });
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/zip", $"novel-{id}.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading novel");
            return StatusCode(500, new { error = "Failed to download novel" });
        }
    }
} 