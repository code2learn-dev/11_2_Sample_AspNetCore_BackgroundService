
using BackgroundServiceSample.WebMinimalApi.Models;
using System.Text.Json;

namespace BackgroundServiceSample.WebMinimalApi.RateServices
{
    /// <summary>
    /// in this class we create new background task using IHostedService 
    /// the lifetime of hostedservice is singleton and we want to store
    /// the task has runned in hosted service that in this example retrieving 
    /// exchange rates to the database by ef core and while the ef core service
    /// is scoped and cannot inject directly into background service class 
    /// and therefor we use IServiceProvice to scope service with CreateScope method
    /// and finally we use ServiceProvider method of scoped service instance to
    /// retrieve scoped service in background service
    /// </summary>
    /// <param name="serviceProvider"></param>
    public class RateEfCoreHostedService(IServiceProvider serviceProvider) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var rateClient = scope.ServiceProvider.GetRequiredService<RateClientService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<RateDbContext>();

                string? currencies = await rateClient.GetClientCurrencies();
                if (!string.IsNullOrEmpty(currencies))
                {
                    var currenciesList = JsonSerializer.Deserialize<IReadOnlyCollection<WebApi.Model.Currency>>(currencies);
                    IEnumerable<Currency> dbContextCurrencyList = currenciesList?.Select(item => new Currency()
                        { 
                            Name = item.Name,
                            Rate = item.Rate
                        }) ?? [];
                    dbContext.Currencies.AddRange(dbContextCurrencyList);
                    await dbContext.SaveChangesAsync(stoppingToken); 
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
    }
}
