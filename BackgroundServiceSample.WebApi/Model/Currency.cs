using System.Text.Json.Serialization;

namespace BackgroundServiceSample.WebApi.Model
{
    public class Currency
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("Rate")]
        public decimal Rate { get; set; }
    }
}
