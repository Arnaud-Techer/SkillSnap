using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using Client.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with API base URL from configuration
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5195";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register services
builder.Services.AddScoped<IPortfolioUserService, PortfolioUserService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Register ViewModels
builder.Services.AddScoped<PortfolioUserViewModel>();
builder.Services.AddScoped<SkillViewModel>();
builder.Services.AddScoped<ProjectViewModel>();

await builder.Build().RunAsync();
