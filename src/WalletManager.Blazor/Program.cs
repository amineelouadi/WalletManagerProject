using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WalletManager.Blazor;
using WalletManager.Blazor.Services;
using WalletManager.Blazor.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var apiUrl = builder.Configuration["apiUrl"] ?? "http://0.0.0.0:8000";
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Server API base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://0.0.0.0:8000") });

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
builder.Configuration["hostUrl"] = "http://0.0.0.0:5000";

// Set up host
var host = builder.Build();

await host.RunAsync();
