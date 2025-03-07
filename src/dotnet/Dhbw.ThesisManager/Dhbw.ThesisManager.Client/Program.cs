using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Dhbw.ThesisManager.Client;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure authentication
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Keycloak", options.ProviderOptions);
    
    // Configure callback paths
    options.ProviderOptions.DefaultScopes.Add("roles");
    options.ProviderOptions.PostLogoutRedirectUri = "/";
    
    // Configure authorization
    options.UserOptions.RoleClaim = "roles";
});

// Register SignOutSessionStateManager as a singleton service
builder.Services.AddSingleton<SignOutSessionStateManager>();

// Add authorization services
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("administrator")));
            
    options.AddPolicy("RequireSupervisorRole", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("supervisor") || 
            context.User.IsInRole("administrator")));
            
    options.AddPolicy("RequireStudentRole", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("student")));
});

// Configure HttpClient with authentication
builder.Services.AddHttpClient("API", client => 
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Add authenticated HttpClient factory
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Add unauthenticated HttpClient for public endpoints
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
