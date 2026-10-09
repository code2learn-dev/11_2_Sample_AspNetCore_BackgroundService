namespace BackgroundServiceSample.WorkerServiceSample.RateServices
{
    public class RateHttpClientService
    {
        private readonly HttpClient _client;

        public RateHttpClientService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:7032/");
        }

        public async Task<string> GetAllCurrenciesAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/rate");
            if (!response.IsSuccessStatusCode) return string.Empty;

            return await response.Content.ReadAsStringAsync();
        }
    }
}
