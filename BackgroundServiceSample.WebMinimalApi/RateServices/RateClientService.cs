namespace BackgroundServiceSample.WebMinimalApi.RateServices
{
    /// <summary>
    /// Create Generic Http client service for rates
    /// </summary>
    public class RateClientService
    {
        private readonly HttpClient _client;

        public RateClientService(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://localhost:7032/");
        }

        public async Task<string?> GetClientCurrencies()
        {
            HttpResponseMessage response = await _client.GetAsync("api/rate");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync(); 
        }
    }
}
