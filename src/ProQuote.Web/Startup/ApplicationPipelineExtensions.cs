using ProQuote.Infrastructure.Data;
using ProQuote.Web.Components;

namespace ProQuote.Web.Startup;

/// <summary>
/// Application pipeline extensions for the web application.
/// </summary>
public static class ApplicationPipelineExtensions
{
    /// <summary>
    /// Seeds ProQuote data according to current environment.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SeedProQuoteAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using IServiceScope scope = app.Services.CreateScope();
        AppDbSeeder seeder = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();
        await seeder.SeedAsync(
            includeSampleData: app.Environment.IsDevelopment(),
            includeDefaultAdmin: app.Environment.IsDevelopment());
    }

    /// <summary>
    /// Configures middleware and endpoints for ProQuote.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication UseProQuoteWebPipeline(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
