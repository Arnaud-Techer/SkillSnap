using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// adding in memory cache
builder.Services.AddMemoryCache();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5210", "https://localhost:7220")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register controllers so attribute-routed controllers are active
builder.Services.AddControllers();

// Adding services to the builder
builder.Services.AddDbContext<SkillSnapContext>(options =>
    options.UseSqlite("Data Source=skillSnap.db"));

// Enable CORS 
builder.Services.AddCors(o => o.AddDefaultPolicy(p => 
        p.WithOrigins("http://localhost:5210")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS
app.UseCors();

// Map attribute-routed controllers
app.MapControllers();

app.UseHttpsRedirection();

app.Run();
