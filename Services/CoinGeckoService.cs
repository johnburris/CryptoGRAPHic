using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CryptoGRAPHic.Models;

namespace CryptoGRAPHic.Services
{
    public class CoinGeckoService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<List<List<double>>> FetchPriceDataAsync()
        {
            var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var pastDate = DateTimeOffset.UtcNow.AddDays(-365).ToUnixTimeSeconds();
            var url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=usd&from={pastDate}&to={currentDate}";

            var response = await client.GetStringAsync(url);
            var marketData = JsonConvert.DeserializeObject<MarketData>(response);

            if (marketData == null || marketData.prices == null)
            {
                throw new Exception("Failed to fetch price data from CoinGecko");
            }

            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(wwwRootPath, "price_actual.json");
            var priceData = JsonConvert.SerializeObject(new { prices = marketData.prices });
            await System.IO.File.WriteAllTextAsync(filePath, priceData);

            return marketData.prices;
        }
    }
}
