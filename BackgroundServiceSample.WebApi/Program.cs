using BackgroundServiceSample.WebApi.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<CurrencyRepository>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseStatusCodePages();
}

app.MapControllers();

app.Run();
