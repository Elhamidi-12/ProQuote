using Microsoft.AspNetCore.Components.Authorization;

using MudBlazor.Services;

using RFQApp.Infrastructure;
using RFQApp.Infrastructure.Data;
using RFQApp.Web.Auth;
using RFQApp.Web.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Infrastructure services (EF Core, Identity, Repositories, Auth)
builder.Services.AddInfrastructure(builder.Configuration);

// Add MudBlazor
builder.Services.AddMudServices();

// Add Authentication State Provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

WebApplication app = builder.Build();

// Seed the database
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbSeeder seeder = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
