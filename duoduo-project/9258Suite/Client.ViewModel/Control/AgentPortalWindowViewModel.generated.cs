




/// <copyright>
/// Copyright ©  2012 Unisys Corporation. All rights reserved. UNISYS CONFIDENTIAL
/// </copyright>

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YoYoStudio.Common.ObjectHistory;
using YoYoStudio.Common.Extensions;
using YoYoStudio.Common.Wpf.ViewModel;
using YoYoStudio.Model.Core;
using YoYoStudio.Model.Chat;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoYoStudio.Model.Media;

namespace YoYoStudio.Client.ViewModel
{
	public partial class AgentPortalWindowViewModel
	{
	

		/// <summary>
		/// Field which backs the AccountBalance property
		/// </summary>
		private HistoryableProperty<string> accountBalance = new HistoryableProperty<string>(string.Empty);

		/// <summary>
		/// Gets / sets the AccountBalance value
		/// </summary>
		[Browsable(false)]
		public  string AccountBalance
		{
			get { return accountBalance.GetValue(); }
			set { ChangeAndNotifyHistory<string>(accountBalance, value, () => AccountBalance ); }
		}
	}
}
	