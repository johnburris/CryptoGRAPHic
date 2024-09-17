using CryptoGRAPHic.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddRazorPages();
builder.Services.AddSingleton<CoinGeckoService>();
builder.Services.AddSingleton<MLService>();

// Configure Kestrel to listen on all IP addresses (0.0.0.0) and port 5000
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // Replace 5000 with your desired port
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Fetch data and train ML model at app launch
var coinGeckoService = app.Services.GetRequiredService<CoinGeckoService>();
var mlService = app.Services.GetRequiredService<MLService>();

var priceData = await coinGeckoService.FetchPriceDataAsync();
mlService.TrainAndPredict(priceData);

app.Run();
