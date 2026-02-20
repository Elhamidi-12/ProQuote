using RFQApp.Infrastructure;
using RFQApp.Infrastructure.Data;
using RFQApp.Web.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Infrastructure services (EF Core, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
