using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;
using WalletManager.Blazor;
using WalletManager.Blazor.Services;
using WalletManager.Blazor.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var apiUrl = "https://localhost:52041"; // Your API URL
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.Converters.Add(new JsonStringEnumConverter());
});



// Server API base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    { "hostUrl", "https://0.0.0.0:5000" },
    { "apiUrl", "https://0.0.0.0:8000" }
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Auth Services
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Add our services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<HttpClientService>();

// Set host URL
builder.Configuration["hostUrl"] = "https://0.0.0.0:5000";

// Set up host
var host = builder.Build();

await host.RunAsync();
