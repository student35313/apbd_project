namespace Project.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        if (fromCurrency.ToUpper() == toCurrency.ToUpper())
            return 1m;
        
        if (!fromCurrency.Equals("PLN", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException("Only PLN as base currency is supported");

        var response = await _httpClient.GetFromJsonAsync<NbpResponse>($"https://api.nbp.pl/api/exchangerates/rates/a/{toCurrency}/?format=json");

        if (response == null || response.Rates == null || !response.Rates.Any())
            throw new Exception("Failed to fetch exchange rate");

        return response.Rates[0].Mid;
    }

    private class NbpResponse
    {
        public List<Rate> Rates { get; set; }

        public class Rate
        {
            public decimal Mid { get; set; }
        }
    }
}