using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Resource;

namespace YoYoStudio.Client.Chat.TemplateSelector
{
    public class AgentPortalTemplateSelector:DataTemplateSelector
    {
        public DataTemplate AgentCommissionTemplate { get; set; }
        public DataTemplate AgentPaymentTemplate { get; set; }
        public DataTemplate BankAccountTemplate { get; set; }
        public DataTemplate BuyDianCardTemplate { get; set; }
        public DataTemplate BuyGuanHaoTemplate { get; set; }
        public DataTemplate BuyMembershipTemplate { get; set; }
        public DataTemplate DepositTemplate { get; set; }
        public DataTemplate DianCardSaleTemplate { get; set; }
        public DataTemplate DianCardStocksTemplate { get; set; }
        public DataTemplate HosterTemplate { get; set; }
        public DataTemplate LogoffTemplate { get; set; }
        public DataTemplate PasswordTemplate { get; set; }
        public DataTemplate UserCashTemplate { get; set; }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            FunctionNodeViewModel node = item as FunctionNodeViewModel;
            if(node != null)
            {
                string pageTitle = node.Title;
                if (string.Compare(pageTitle, Text.AgentCommission) == 0)
                    return AgentCommissionTemplate;
                else if (string.Compare(pageTitle, Text.AgentPayment) == 0)
                    return AgentPaymentTemplate;
                else if (string.Compare(pageTitle, Text.BankAccount) == 0)
                    return BankAccountTemplate;
                else if (string.Compare(pageTitle, Text.BuyDianCard) == 0)
                    return BuyDianCardTemplate;
                else if (string.Compare(pageTitle, Text.BuyGuanHao) == 0)
                    return BuyGuanHaoTemplate;
                else if (string.Compare(pageTitle, Text.BuyMembership) == 0)
                    return BuyMembershipTemplate;
                else if (string.Compare(pageTitle, Text.Deposit) == 0)
                    return DepositTemplate;
                else if (string.Compare(pageTitle, Text.DianCardSale) == 0)
                    return DianCardSaleTemplate;
                else if (string.Compare(pageTitle, Text.DianCardStocks) == 0)
                    return DianCardStocksTemplate;
                else if (string.Compare(pageTitle, Text.Hoster) == 0)
                    return HosterTemplate;
                else if (string.Compare(pageTitle, Text.LogoffExit) == 0)
                    return LogoffTemplate;
                else if (string.Compare(pageTitle, Text.PasswordChange) == 0)
                    return PasswordTemplate;
                else if (string.Compare(pageTitle, Text.UserCash) == 0)
                    return UserCashTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
