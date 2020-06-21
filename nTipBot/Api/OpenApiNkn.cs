using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using nTipBot.Models.OpenApiNkn;
using System.Net.Http;
using System.Threading.Tasks;

namespace nTipBot.Api
{
	public class OpenApiNkn
	{
		public static async Task<AddressTransactionResponse> GetFaucetTx(string address)
		{
			HttpClient httpClient = new HttpClient();
			string content = await httpClient.GetStringAsync($"https://openapi.nkn.org/api/v1/addresses/{address}/transactions?per_page={100}&page={1}");
			return await Task.Run(() => JsonConvert.DeserializeObject<AddressTransactionResponse>(content));
		}
		public static async Task<decimal> GetNKNRate()
		{
			HttpClient httpClient = new HttpClient();
			string content = await httpClient.GetStringAsync("https://price.nknx.org/price?quote=NKN&currency=USD");
			return await Task.Run(() => JArray.Parse(content)[0]["prices"][0]["price"].Value<decimal>());
		}
	}
}
