using nTipBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace nTipBot.Models
{
	public class UserDialogModel
	{
		public UserDialogType DialogType { get; set; }
		public decimal Amount { get; set; }
		public string Address { get; set; }
	}
}
