using InventaiNovel;
using Inventai.TextAgents;
using Inventai.ImageAgents;
using Microsoft.EntityFrameworkCore;
using web.Server.Data;
using web.Server.Models;

namespace web.Server.Services
{
    public class NovelGenerationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NovelGenerationService> _logger;

        public NovelGenerationService(IServiceProvider serviceProvider, ILogger<NovelGenerationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Get pending generations
                    var pendingGenerations = await dbContext.NovelGenerations
                        .Where(g => g.Status == "Pending")
                        .ToListAsync(stoppingToken);

                    foreach (var generation in pendingGenerations)
                    {
                        try
                        {
                            // Update status to in progress
                            generation.Status = "InProgress";
                            await dbContext.SaveChangesAsync(stoppingToken);

                            // Create agents
                            var textAgent = new TextAgentOpenAI("gpt-3.5-turbo", generation.Credentials.OpenAiApiKey);
                            var imageAgent = new ImageAgentStableDiffusion(
                                "https://api.segmind.com/v1/stable-diffusion-3.5-turbo-txt2img", 
                                generation.Credentials.SegmindApiKey);

                            // Create and save novel
                            var novelManager = new InventaiNovelManager(textAgent, imageAgent);
                            novelManager.CreateNovel(generation.Request);

                            var homePath = Environment.GetEnvironmentVariable("HOME");
                            var basePath = string.IsNullOrEmpty(homePath)
                                ? Path.Combine(Directory.GetCurrentDirectory(), "novels", generation.Id.ToString())  // For local development
                                : Path.Combine(homePath, "site", "wwwroot", "novels", generation.Id.ToString());   // For Azure

                            _logger.LogInformation("Saving novel to path: {BasePath}", basePath);

                            // Ensure the directory exists
                            if (!Directory.Exists(basePath))
                            {
                                _logger.LogInformation("Creating directory: {BasePath}", basePath);
                                Directory.CreateDirectory(basePath);
                            }

                            await novelManager.SaveNovel(basePath);
                            await novelManager.ExportToRenpy(basePath);

                            // Verify files were created
                            var novelFiles = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);
                            _logger.LogInformation("Created {Count} files in {BasePath}: {Files}", 
                                novelFiles.Length, 
                                basePath,
                                string.Join(", ", novelFiles));

                            // Update status to completed
                            generation.Status = "Completed";
                            generation.OutputPath = basePath;
                            generation.CompletedAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error generating novel {Id}", generation.Id);
                            generation.Status = "Failed";
                            generation.ErrorMessage = ex.Message;
                            generation.CompletedAt = DateTime.UtcNow;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in novel generation service");
                }

                // Wait before checking for new generations
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
} 