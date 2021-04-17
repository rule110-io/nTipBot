using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using nTipBot.Models;
using nTipBot.Models.OpenApiNkn;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace nTipBot.Api
{
	public class OpenApiNkn
	{
		public static HttpClient httpClient = new HttpClient();

		public static async Task<AddressTransactionResponse> GetFaucetTx(string address)
		{
			string content = await httpClient.GetStringAsync($"https://openapi.nkn.org/api/v1/addresses/{address}/transactions?per_page={100}&page={1}");
			return await Task.Run(() => JsonConvert.DeserializeObject<AddressTransactionResponse>(content));
		}
		public static async Task<decimal> GetNKNRate()
		{
			HttpClient httpClient = new HttpClient();
			string content = await httpClient.GetStringAsync("https://api.binance.com/api/v3/ticker/price?symbol=NKNUSDT");
			return await Task.Run(() => decimal.Parse(JObject.Parse(content).Value<string>("price")));
		}

		public static async Task<Tuple<string, string>> GetBinanceUSDAndBTCRate()
		{
			var usdTask = GetBinanceNKNUSDTRate();
			var btcTask = GetBinanceNKNBTCRate();

			var waitTask = Task.WhenAll(usdTask, btcTask);

			await waitTask;

			if (waitTask.Status == TaskStatus.RanToCompletion)
			{
				return new Tuple<string, string>(usdTask.Result, btcTask.Result);
			}
			else
			{
				throw new Exception("Failed to retrieve nkn price information.");
			}
		}

		public static async Task<string> GetBinanceNKNUSDTRate()
		{
			string content = await httpClient.GetStringAsync("https://api.binance.com/api/v3/ticker/price?symbol=NKNUSDT");
			return await Task.Run(() => JObject.Parse(content).Value<string>("price"));
		}
		public static async Task<string> GetBinanceNKNBTCRate()
		{
			string content = await httpClient.GetStringAsync("https://api.binance.com/api/v3/ticker/price?symbol=NKNBTC");
			return await Task.Run(() => JObject.Parse(content).Value<string>("price"));
		}
		public static async Task<NetworkStatusModel> GetNetworkStats()
		{
			string content = await httpClient.GetStringAsync($"https://api.nknx.org/network/stats");
			return await Task.Run(() => JsonConvert.DeserializeObject<NetworkStatusModel>(content));
		}

		public static async Task<NKNGeoSummary> GetNetworkStatsNKNOrg()
		{
			string content = await httpClient.GetStringAsync($"https://api.nkn.org/v1/geo/summary");
			return await Task.Run(() => JsonConvert.DeserializeObject<NKNGeoSummary>(content));
		}
		public static async Task<NKNBlockchainStats> GetBlockchainStats()
		{
			string content = await httpClient.GetStringAsync($"https://openapi.nkn.org/api/v1/statistics/counts");
			return await Task.Run(() => JsonConvert.DeserializeObject<NKNBlockchainStats>(content));
		}

	}
}
