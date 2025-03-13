using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Dhbw.ThesisManager.Client;
using Dhbw.ThesisManager.UserClient;
using Dhbw.ThesisManager.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register HttpClient
builder.Services.AddScoped(sp => new System.Net.Http.HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register API clients
builder.Services.AddScoped<Client>();
builder.Services.AddScoped<UserClient>();
builder.Services.AddScoped<RegistrationClient>();

// Register authentication services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
