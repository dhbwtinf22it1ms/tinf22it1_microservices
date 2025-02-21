using Dhbw.ThesisManager.Api;
using Dhbw.ThesisManager.Api.Configuration;
using Dhbw.ThesisManager.Api.Components;
using Dhbw.ThesisManager.Api.Data;
using EasyNetQ;
using Dhbw.ThesisManager.Api.Data.Mapping;
using Dhbw.ThesisManager.Api.Services;
using Dhbw.ThesisManager.Client.Pages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add DbContext
builder.Services.AddDbContext<ThesisManagerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Keycloak
builder.Services.Configure<KeycloakOptions>(
    builder.Configuration.GetSection(KeycloakOptions.Section));

// Add HttpClient for Keycloak
builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();

// Add Controllers
builder.Services.AddControllers();

// Configure EasyNetQ and Event Publishing
builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus("host=localhost"));
builder.Services.AddScoped<IEventPublisher, EventPublisher>();

// Add Interface Implementations
builder.Services.AddScoped<IThesisController, ThesisControllerImpl>();
builder.Services.AddScoped<IUserController, UserControllerImpl>();
builder.Services.AddScoped<IRegistrationController, RegistrationControllerImpl>();
builder.Services.AddScoped<IKeycloakService, KeycloakService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Dhbw.ThesisManager.Client._Imports).Assembly);

app.MapControllers();

app.Run();
