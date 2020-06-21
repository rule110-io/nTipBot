using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace nTipBot.Api
{
	internal class ExchangeRatesApi
	{
		public static async Task<decimal> GetUSDToEur()
		{
			HttpClient httpClient = new HttpClient();
			string content = await httpClient.GetStringAsync("https://api.exchangeratesapi.io/latest?base=USD&symbols=EUR");
			return await Task.Run(() => JObject.Parse(content)["rates"]["EUR"].Value<decimal>());
		}

		public static async Task<decimal> GetUSDToGBP()
		{
			HttpClient httpClient = new HttpClient();
			string content = await httpClient.GetStringAsync("https://api.exchangeratesapi.io/latest?base=USD&symbols=GBP");
			return await Task.Run(() => JObject.Parse(content)["rates"]["GBP"].Value<decimal>());
		}
	}
}
