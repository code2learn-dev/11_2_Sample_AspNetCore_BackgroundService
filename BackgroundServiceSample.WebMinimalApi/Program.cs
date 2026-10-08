using BackgroundServiceSample.WebMinimalApi.Helpers;
using BackgroundServiceSample.WebMinimalApi.Models;
using BackgroundServiceSample.WebMinimalApi.RateServices;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// adding In-Memory EF Core
builder.Services.AddDbContext<RateDbContext>(
		options => options.UseInMemoryDatabase("rate_db"));

// adding memory cache service for caching exchange rates
builder.Services.AddMemoryCache();
// adding Http client service for remote API rates
builder.Services.AddHttpClient<RateClientService>();
// define cache service as singleton because IHosted Service work in singleton state
builder.Services.AddSingleton<CacheService>();
// adding IHostedService for creating jon that running in background
// it's mean create background task with IHostedService 
//builder.Services.AddHostedService<RateHostedService>();


// add Hosted Service for store exchange rates periodically in specified time period
// in database with EF Core service 
builder.Services.AddHostedService<RateEfCoreHostedService>();
// using razor page service to display rates list
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStatusCodePages();

// add static file middleware
app.UseStaticFiles();

// add razor page midlleware
app.MapRazorPages();

//app.MapGet("/", () => "Exchange Rates");

// getting exchange rated list from cache in minimal API
app.MapGet("/rates", async (
	HttpContext context,
	CacheService cacheService) =>
{
	var currencies = cacheService.GetCacheData("rates");
	if (!string.IsNullOrEmpty(currencies))
	{
		context.Response.ContentType = MediaTypeNames.Application.Json;

		await context.Response.WriteAsync(currencies);
	}
	else 
    {
		context.Response.ContentType = MediaTypeNames.Text.Plain;
        await context.Response.WriteAsync("Not set any currency");
    }
});

app.Run();
