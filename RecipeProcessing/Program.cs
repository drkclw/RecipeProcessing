using Azure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RecipeProcessing.Data;
using RecipeProcessing.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<ImageService>();
builder.Services.AddTransient<TextAnalyticsService>();
builder.Services.AddTransient<SpeechService>();
builder.Services.AddTransient<LuisService>();

string? cs = Environment.GetEnvironmentVariable("RecExtConfigConnectionString");
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var settings = config.Build();

    config.AddAzureAppConfiguration(options =>
    {
        options.Connect(cs)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(new DefaultAzureCredential());
                });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
