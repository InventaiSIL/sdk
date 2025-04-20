using web.Server.Services;
using Microsoft.EntityFrameworkCore;
using web.Server.Data;

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
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // For Azure deployment, use environment variable
        connectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection");
    }
    
    // Ensure the database directory exists
    var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
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
app.UseStaticFiles();
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
