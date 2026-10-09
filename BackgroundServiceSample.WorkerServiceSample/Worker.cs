using BackgroundServiceSample.WorkerServiceSample.Models;
using BackgroundServiceSample.WorkerServiceSample.RateServices;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BackgroundServiceSample.WorkerServiceSample
{
    /// <summary>
    /// notice to the background service must running in the period of times
    /// and be like previous background services
    /// </summary>
    /// <param name="logger"></param>
    public class Worker(
        ILogger<Worker> logger, 
        IServiceProvider serviceProvider) : BackgroundService
    { 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                using var scope = serviceProvider.CreateScope();
                var rateClient = scope.ServiceProvider.GetRequiredService<RateHttpClientService>();
                var context = scope.ServiceProvider.GetRequiredService<RateWorkerServiceDbContext>();

                var currenciesResponce = await rateClient.GetAllCurrenciesAsync();
                var currenciesApiModel = JsonSerializer.Deserialize<IReadOnlyCollection<WebApi.Model.Currency>>(currenciesResponce);
                IEnumerable<Currency> currencies = currenciesApiModel?.Select(c => new Currency()
                {
                    Name = c.Name,
                    Rate = c.Rate
                }) ?? [];

                await context.AddRangeAsync(currencies);
                await context.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
    }
}
