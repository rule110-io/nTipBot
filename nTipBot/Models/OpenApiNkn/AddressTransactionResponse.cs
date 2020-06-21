using Newtonsoft.Json;
using System.Collections.Generic;

namespace nTipBot.Models.OpenApiNkn
{
	public class Payload
	{

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("transaction_id")]
		public int TransactionId { get; set; }

		[JsonProperty("payloadType")]
		public string PayloadType { get; set; }

		[JsonProperty("sender")]
		public string Sender { get; set; }

		[JsonProperty("senderWallet")]
		public string SenderWallet { get; set; }

		[JsonProperty("recipient")]
		public string Recipient { get; set; }

		[JsonProperty("recipientWallet")]
		public string RecipientWallet { get; set; }

		[JsonProperty("amount")]
		public decimal Amount { get; set; }

		[JsonProperty("submitter")]
		public object Submitter { get; set; }

		[JsonProperty("registrant")]
		public object Registrant { get; set; }

		[JsonProperty("registrantWallet")]
		public object RegistrantWallet { get; set; }

		[JsonProperty("name")]
		public object Name { get; set; }

		[JsonProperty("subscriber")]
		public object Subscriber { get; set; }

		[JsonProperty("identifier")]
		public object Identifier { get; set; }

		[JsonProperty("topic")]
		public object Topic { get; set; }

		[JsonProperty("bucket")]
		public object Bucket { get; set; }

		[JsonProperty("duration")]
		public object Duration { get; set; }

		[JsonProperty("meta")]
		public object Meta { get; set; }

		[JsonProperty("public_key")]
		public object PublicKey { get; set; }

		[JsonProperty("registration_fee")]
		public object RegistrationFee { get; set; }

		[JsonProperty("nonce")]
		public object Nonce { get; set; }

		[JsonProperty("txn_expiration")]
		public object TxnExpiration { get; set; }

		[JsonProperty("symbol")]
		public object Symbol { get; set; }

		[JsonProperty("total_supply")]
		public object TotalSupply { get; set; }

		[JsonProperty("precision")]
		public object Precision { get; set; }

		[JsonProperty("nano_pay_expiration")]
		public object NanoPayExpiration { get; set; }

		[JsonProperty("signerPk")]
		public string SignerPk { get; set; }

		[JsonProperty("added_at")]
		public string AddedAt { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("generateWallet")]
		public object GenerateWallet { get; set; }

		[JsonProperty("subscriberWallet")]
		public object SubscriberWallet { get; set; }

		[JsonProperty("sigchain")]
		public object Sigchain { get; set; }
	}

	public class Data
	{

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("block_id")]
		public int BlockId { get; set; }

		[JsonProperty("attributes")]
		public string Attributes { get; set; }

		[JsonProperty("fee")]
		public int Fee { get; set; }

		[JsonProperty("hash")]
		public string Hash { get; set; }

		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		[JsonProperty("txType")]
		public string TxType { get; set; }

		[JsonProperty("block_height")]
		public int BlockHeight { get; set; }

		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }

		[JsonProperty("payload")]
		public Payload Payload { get; set; }

		[JsonProperty("programs")]
		public IList<object> Programs { get; set; }
	}

	public class AddressTransactionResponse
	{

		[JsonProperty("current_page")]
		public int CurrentPage { get; set; }

		[JsonProperty("data")]
		public IList<Data> Data { get; set; }

		[JsonProperty("first_page_url")]
		public string FirstPageUrl { get; set; }

		[JsonProperty("from")]
		public int? From { get; set; }

		[JsonProperty("next_page_url")]
		public string NextPageUrl { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("per_page")]
		public string PerPage { get; set; }

		[JsonProperty("prev_page_url")]
		public string PrevPageUrl { get; set; }

		[JsonProperty("to")]
		public int? To { get; set; }
	}


}
