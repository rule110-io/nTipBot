using NknSdk.Common.Options;
using NknSdk.Common.Rpc.Results;
using NknSdk.Wallet;
using nTipBot.Api;
using nTipBot.Models.OpenApiNkn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace nTipBot
{
	internal class Program
	{
		private static readonly string apiToken = ApiKeys.TelegramBotApiToken;
		private static Guid secretSalt = Guid.Parse(ApiKeys.SecretSalt);
		private static readonly string faucetSeed = ApiKeys.FaucetSeed;
		private static readonly string faucetAddress = ApiKeys.FaucetAddress;
		private static readonly SemaphoreSlim semaphoreFaucet = new SemaphoreSlim(1, 1);

		private static TelegramBotClient client;

		private static readonly Dictionary<int, DateTime> faucetCache = new Dictionary<int, DateTime>();

		private static string BotWelcomeMessage => @$"Welcome to nTipBot\!

Use the buttons to interact with all the features the bot has to offer\.
To tip a user make sure you have enough balance, then simply reply `/tip amount` to a message in any chat that has nTipBot\.

The faucet address is `{faucetAddress}`, when claiming the faucet you will be given a share of the total balance, feel free to donate to the faucet address\.

⚠ Even though this bot stores no information whatsoever about your interactions, nor does it store any seed or wallet information, the wallets are subject to potential telegram exploits and deterministical wallet generation could be compromised\. We can not guarantee safety of your funds\! ⚠";

		private static string BotInfoMessage => $@"👷‍♂ How does it work?
The wallet seed is derived from your telegram user, this way everyone that has a telegram user automatically has a wallet associated with their account and can send and receive tips\.
When you send or receive a tip for both the sender and the recipients the wallets are derived and the transaction is signed on the server\.

⚠ Is it safe?
Because the seed is derived from your telegram user it is in theory possible to hijack the telegram api and pose as another user to gain access to your funds through the bot, alternatively it is possible the technique for the derivation is brute forced\.
That is why we urge users to only use small quantities of NKN as we can not guarantee the safety of your funds\.

🚰 How does the faucet work?
Every telegram user can periodically claim from the faucet, the amount of NKN you receive is 1/1000th of the total balance of the faucet at time of transfer\.

👩‍💻 Github
All code is opensource and can be found here [Github](https://github.com/rule110-io/nTipBot/)

❔ Reach out
If you have any further questions you can each me here @MutsiMutsi";

		private static void Main(string[] args)
		{
			client = new TelegramBotClient(apiToken);
			client.OnMessage += Client_OnMessage;
			client.StartReceiving();

			AddressTransactionResponse tx = OpenApiNkn.GetFaucetTx(faucetAddress).GetAwaiter().GetResult();

			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("EN-US");

			Console.WriteLine("Bot is alive!");
			Console.ReadLine();
		}

		private static async void Client_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Message.Text == null)
			{
				return;
			}

			try
			{
				await ProcessMessage(e);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await client.SendTextMessageAsync(e.Message.Chat.Id, Util.MarkdownV2Escape($"Error occurred: {ex.Message}, try again or contact @MutsiMutsi"), ParseMode.Default);
			}
		}

		private static async Task ProcessMessage(MessageEventArgs e)
		{
			ReplyKeyboardMarkup rkm = GenerateKeboard();

			if (e.Message.ReplyToMessage != null)
			{
				if (e.Message.Text.StartsWith("/tip"))
				{
					//Block tipping the channel
					if (e.Message.Chat.Id != e.Message.ReplyToMessage.From.Id)
					{
						await TipCommand(e);
					}
					else
					{
						await client.SendTextMessageAsync(e.Message.Chat.Id, "You can't a channel", ParseMode.MarkdownV2);
					}
				}
				return;
			}
			else if (e.Message.Chat.Id == e.Message.From.Id)
			{
				Console.WriteLine($"[{DateTime.Now.ToString()}] {e.Message.Text}");
				await HandleButtonCommands(e, rkm);
			}
		}

		private static async Task TipCommand(MessageEventArgs e)
		{
			Wallet userWallet = GetUserWallet(e.Message.From);
			Wallet replyToWallet = GetUserWallet(e.Message.ReplyToMessage.From);
			Console.WriteLine($"[{DateTime.Now.ToString()}] {e.Message.Text}");
			if (userWallet.Address == replyToWallet.Address)
			{
				await client.SendTextMessageAsync(e.Message.Chat.Id, "You can't tip yourself", ParseMode.MarkdownV2);
				return;
			}
			string[] cmdSplit = e.Message.Text.Replace("/tip ", "").Split();
			string amountStr = cmdSplit[0];

			bool inUsd = amountStr[0] == '$';
			bool inEur = amountStr[0] == '€';
			bool inGbp = amountStr.StartsWith("£");
			if (inUsd)
			{
				amountStr = amountStr.Replace("$", "");
			}
			if (inEur)
			{
				amountStr = amountStr.Replace("€", "");
			}
			if (inGbp)
			{
				amountStr = amountStr.Replace("£", "");
			}

			if (decimal.TryParse(amountStr, out decimal amount))
			{

				if (inUsd)
				{
					amount /= await OpenApiNkn.GetNKNRate();
				}
				if (inEur)
				{
					amount /= await OpenApiNkn.GetNKNRate();
					amount /= await ExchangeRatesApi.GetUSDToEur();
				}
				if (inGbp)
				{
					amount /= await OpenApiNkn.GetNKNRate();
					amount /= await ExchangeRatesApi.GetUSDToGBP();
				}
				amount = decimal.Round(amount, 8);

				if (amount < 0.00000001m)
				{
					await client.SendTextMessageAsync(e.Message.Chat.Id, "Minimum tip is 0\\.00000001 NKN", ParseMode.MarkdownV2);
					return;
				}
				if ((await userWallet.GetBalanceAsync()).Amount >= amount)
				{
					TransactionOptions options = new TransactionOptions() { Fee = 0, Attributes = "" };
					string txnHash = await userWallet.TransferToAsync(replyToWallet.Address, amount, options);

					string nscanUrl = $"🔗 [nScan\\.io](https://nscan.io/transactions/{txnHash})";
					string msg = $"{Util.MarkdownV2Escape(e.Message.From.FirstName)} tipped {Util.MarkdownV2Escape(e.Message.ReplyToMessage.From.FirstName)} `{amount}` NKN\r\n{nscanUrl}";
					await client.SendTextMessageAsync(e.Message.Chat.Id, msg, ParseMode.MarkdownV2, true);
				}
				else
				{
					await client.SendTextMessageAsync(e.Message.Chat.Id, $"{Util.MarkdownV2Escape(e.Message.From.FirstName)} not enough balance to tip `{amount}`", ParseMode.MarkdownV2);
				}
			}
			else
			{
				await client.SendTextMessageAsync(e.Message.Chat.Id, $"{Util.MarkdownV2Escape(e.Message.From.FirstName)} invalid amount try the following syntax: `/tip 0.001`", ParseMode.MarkdownV2);
			}
		}

		private static async Task HandleButtonCommands(MessageEventArgs e, ReplyKeyboardMarkup rkm)
		{
			switch (e.Message.Text)
			{
				case "💳 My Address":
					{
						Wallet userWallet = GetUserWallet(e.Message.From);
						string nscanUrl = $"🔗 [nScan\\.io](https://nscan.io/addresses/{userWallet.Address})";

						string msg = $"`{userWallet.Address}`" + $"\r\n{nscanUrl}";
						await client.SendTextMessageAsync(e.Message.Chat.Id, $"Wallet address\r\n{msg}", ParseMode.MarkdownV2, true, false, 0, rkm);
						break;
					}
				case "💰 Balance":
					GetBalanceResult balanceResult = await GetUserWallet(e.Message.From).GetBalanceAsync();
					await client.SendTextMessageAsync(e.Message.Chat.Id, $"Your balance\r\n`{balanceResult.Amount.ToString()} NKN`", ParseMode.MarkdownV2, false, false, 0, rkm);
					break;
				case "🌱 Export Seed":
					{
						string msg = GetUserWallet(e.Message.From).Seed;
						await client.SendTextMessageAsync(e.Message.Chat.Id, $"Wallet seed\r\n`{msg}`", ParseMode.MarkdownV2, true, false, 0, rkm);
						break;
					}
				case "🚰 Claim Faucet":
					{
						string faucetMsg = await UseFaucet(e.Message.From);
						await client.SendTextMessageAsync(e.Message.Chat.Id, faucetMsg, ParseMode.MarkdownV2, true, false, 0, rkm);
						break;
					}
				case "💸 Donate 1 NKN to Faucet":
					{
						string donateReply;
						Wallet userWallet = GetUserWallet(e.Message.From);
						if ((await userWallet.GetBalanceAsync()).Amount >= 1)
						{
							TransactionOptions options = new TransactionOptions() { Fee = 0, Attributes = "" };
							try
							{
								string txnHash = await userWallet.TransferToAsync(faucetAddress, 1, options);
								donateReply = $"Thank you for your contribution\\!\r\nTransaction hash: `{txnHash}`";
							}
							catch (Exception ex)
							{
								donateReply = $"Donation failed: {ex.Message}";
							}
						}
						else
						{
							donateReply = "You do not have 1 NKN to donate\\.";
						}
						await client.SendTextMessageAsync(e.Message.Chat.Id, donateReply, ParseMode.MarkdownV2, true, false, 0, rkm);
						break;
					}
				case "❓ Info":
					await client.SendTextMessageAsync(e.Message.Chat.Id, BotInfoMessage, ParseMode.MarkdownV2, false, false, 0, rkm);
					break;
				case "/start":
					await client.SendTextMessageAsync(e.Message.Chat.Id, BotWelcomeMessage, ParseMode.MarkdownV2, false, false, 0, rkm);
					break;
				default:
					await client.SendTextMessageAsync(e.Message.Chat.Id, "Try one of the buttons instead.", ParseMode.Default, true, false, 0, rkm);
					break;
			}
		}

		private static ReplyKeyboardMarkup GenerateKeboard()
		{
			return new ReplyKeyboardMarkup
			{
				Keyboard =
					new KeyboardButton[][]
					{
						new KeyboardButton[]
						{
							new KeyboardButton("💳 My Address"),
							new KeyboardButton("💰 Balance"),
						},
						new KeyboardButton[]
						{
							new KeyboardButton("💸 Donate 1 NKN to Faucet"),
							new KeyboardButton("🚰 Claim Faucet"),
						},
						new KeyboardButton[]
						{
							new KeyboardButton("🌱 Export Seed"),
						}
						,
						new KeyboardButton[]
						{
							new KeyboardButton("❓ Info"),
						}
					}
			};
		}

		private static Wallet GetUserWallet(User user)
		{
			string userHash = Util.ComputeSha256Hash($"{user.Id}{secretSalt}");
			return new Wallet(new NknSdk.Common.Options.WalletOptions
			{
				SeedHex = userHash
			});
		}

		private static async Task<string> UseFaucet(User user)
		{
			if (faucetCache.ContainsKey(user.Id))
			{
				TimeSpan elapsed = DateTime.Now - faucetCache[user.Id];
				if (elapsed < new TimeSpan(0, 0, 7))
				{
					return $"You've already used the faucet recently\\.";
				}
			}
			//Check if faucet already granted this wallet
			Wallet userWallet = GetUserWallet(user);
			AddressTransactionResponse faucetTx = await OpenApiNkn.GetFaucetTx(faucetAddress);
			if (faucetTx.Data.Any(o => o.Payload.RecipientWallet.Trim().ToLower() == userWallet.Address.Trim().ToLower()))
			{
				return $"You've already used the faucet recently\\.";
			}

			await semaphoreFaucet.WaitAsync();
			try
			{
				//Valid recipient, proceed with transaction.
				Wallet faucetWallet = new Wallet(new WalletOptions
				{
					SeedHex = faucetSeed
				});

				decimal balance = (await faucetWallet.GetBalanceAsync()).Amount;

				if (balance < 0.0001m)
				{
					return "Faucet ran dry, try again later\\.";
				}

				decimal amount = balance / 1000;
				amount = decimal.Round(amount, 8);

				TransactionOptions options = new TransactionOptions() { Fee = 0, Attributes = "" };
				string txnHash = await faucetWallet.TransferToAsync(userWallet.Address, amount, options);

				faucetCache.Add(user.Id, DateTime.Now);

				string nscanUrl = $"🔗 [nScan\\.io](https://nscan.io/transactions/{txnHash})";
				return $"You've received `{amount}` NKN\r\nTransaction hash: `{txnHash}`\r\n{nscanUrl}";
			}
			finally
			{
				semaphoreFaucet.Release();
			}
		}
	}
}
