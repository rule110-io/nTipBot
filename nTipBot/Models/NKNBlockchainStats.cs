using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace nTipBot.Models
{
	public class NKNBlockchainStats
	{
		[JsonProperty("blockCount")]
		public int BlockCount { get; set; }

		[JsonProperty("txCount")]
		public int TxCount { get; set; }

		[JsonProperty("addressCount")]
		public int AddressCount { get; set; }
	}
}
