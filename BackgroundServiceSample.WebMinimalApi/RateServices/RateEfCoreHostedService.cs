
using BackgroundServiceSample.WebMinimalApi.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackgroundServiceSample.WebMinimalApi.RateServices
{
	/// <summary>
	/// in this class we create new background task using IHostedService 
	/// the lifetime of hostedservice is singleton and we want to store
	/// the task has runned in hosted service that in this example retrieving 
	/// exchange rates to the database by ef core and while the ef core service
	/// is scoped and cannot inject directly into background service class 
	/// and therefor we use IServiceProvider to scope service with CreateScope method
	/// and finally we use ServiceProvider method of scoped service instance to
	/// retrieve scoped service in background service
	/// </summary>
	/// <param name="serviceProvider"></param>
	public class RateEfCoreHostedService(IServiceProvider serviceProvider) : BackgroundService
	{

		/// <summary>
		/// using of this method to ensure rates fetched from client and 
		/// store in database with EF Core before process the first request
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			bool isSuccess = false;
			while (!isSuccess && !cancellationToken.IsCancellationRequested)
			{
				isSuccess = await TryStoreRatedInDbContext(cancellationToken);
			}

			await base.StartAsync(cancellationToken);
		}

		/// <summary>
		/// implement the abstract base BackgroundService method to perform
		/// periodically specified task in the blocks of time until application has stopped
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
				await TryStoreRatedInDbContext(stoppingToken);
			}
		}


		private async Task<bool> TryStoreRatedInDbContext(CancellationToken cancellationToken)
		{
			try
			{
				using var scope = serviceProvider.CreateScope();
				var rateClinet = scope.ServiceProvider.GetRequiredService<RateClientService>();
				var dbContext = scope.ServiceProvider.GetRequiredService<RateDbContext>();

				var currencies = await rateClinet.GetClientCurrencies();
				if (!string.IsNullOrEmpty(currencies))
				{
					var currenciesList = JsonSerializer.Deserialize<IReadOnlyCollection<WebApi.Model.Currency>>(currencies);
					IEnumerable<Currency> dbContextCurrencyList =
						currenciesList?.Select(c => new Currency()
						{
							Name = c.Name,
							Rate = c.Rate
						}) ?? [];
					await dbContext.Currencies.AddRangeAsync(dbContextCurrencyList);
					await dbContext.SaveChangesAsync(cancellationToken);
					return true;
				}

				return false;
			}
			catch
			{
				return false;
			}
		}
	}
}
