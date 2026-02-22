using ProQuote.Web.Startup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProQuoteWebServices(builder.Configuration);

WebApplication app = builder.Build();
await app.SeedProQuoteAsync();
app.UseProQuoteWebPipeline();
app.Run();
