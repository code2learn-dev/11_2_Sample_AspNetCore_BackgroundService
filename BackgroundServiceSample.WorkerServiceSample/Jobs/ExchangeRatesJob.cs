using BackgroundServiceSample.WorkerServiceSample.Models;
using BackgroundServiceSample.WorkerServiceSample.RateServices;
using Quartz;
using System.Text.Json;

namespace BackgroundServiceSample.WorkerServiceSample.Jobs
{
    public class ExchangeRatesJob : IJob
    {
        private readonly RateWorkerServiceDbContext _context;
        private readonly RateHttpClientService _rateService;
        private readonly ILogger<ExchangeRatesJob> _logger;

        public ExchangeRatesJob(
            RateWorkerServiceDbContext context,
            RateHttpClientService rateService,
            ILogger<ExchangeRatesJob> logger)
        {
            _context = context;
            _rateService = rateService;
            _logger = logger;
        }

        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching latest rates");
            string currenciesResponse = await _rateService.GetAllCurrenciesAsync();
            if (string.IsNullOrEmpty(currenciesResponse)) return;

            var apiCurrencies = JsonSerializer.Deserialize<IReadOnlyCollection<WebApi.Model.Currency>>(currenciesResponse);
            IEnumerable<Currency> currencies = apiCurrencies.Select(c => new Currency()
            {
                Name = c.Name,
                Rate = c.Rate
            });
            await _context.AddRangeAsync(currencies);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Api Currencies stores in database");
        }
    }
}
