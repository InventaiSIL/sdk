using web.Server.Services;
using Microsoft.EntityFrameworkCore;
using web.Server.Data;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("OpenAiApiKey", "SegmindApiKey")
              .AllowCredentials();
    });
});

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Get the HOME directory for Azure App Service
    var homePath = Environment.GetEnvironmentVariable("HOME");
    var dbPath = string.IsNullOrEmpty(homePath) 
        ? Path.Combine(Directory.GetCurrentDirectory(), "Data")  // For local development
        : Path.Combine(homePath, "site", "wwwroot", "Data");    // For Azure
    
    // Ensure the database directory exists
    if (!Directory.Exists(dbPath))
    {
        Directory.CreateDirectory(dbPath);
    }
    
    // Use a fixed path for the SQLite database
    var dbFilePath = Path.Combine(dbPath, "novels.db");
    options.UseSqlite($"Data Source={dbFilePath}");
});

// Add background service
builder.Services.AddHostedService<NovelGenerationService>();

var app = builder.Build();

// Use CORS before other middleware
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Configure static files
var homePath = Environment.GetEnvironmentVariable("HOME");
var novelsPath = string.IsNullOrEmpty(homePath)
    ? Path.Combine(Directory.GetCurrentDirectory(), "novels")  // For local development
    : Path.Combine(homePath, "site", "wwwroot", "novels");    // For Azure

if (!Directory.Exists(novelsPath))
{
    Directory.CreateDirectory(novelsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(novelsPath),
    RequestPath = "/novels",
    ServeUnknownFileTypes = true
});

app.UseDefaultFiles();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

// Apply migrations
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
}

app.Run();
