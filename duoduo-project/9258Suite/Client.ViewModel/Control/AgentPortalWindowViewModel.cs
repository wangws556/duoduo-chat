using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using YoYoStudio.Common.Wpf.ViewModel;
using YoYoStudio.Model.Core;
using YoYoStudio.Resource;
using Snippets;
using System.Runtime.InteropServices;

namespace YoYoStudio.Client.ViewModel
{
    [ComVisible(true)]
    [SnippetPropertyINPC(type="string", field="accountBalance", property="AccountBalance", defaultValue="string.Empty")]
    public partial class AgentPortalWindowViewModel:WindowViewModel
    {
        public string AgentCommissionLabel { get; set; }
        //public string AgentPaymentLabel { get; set; }
        public string BankAccountLabel { get; set; }
        public string BuyDianCardLabel { get; set; }
        public string BuyGuanHaoLabel { get; set; }
        public string BuyMembershipLabel { get; set; }
        public string DepositLabel { get; set; }
        public string DianCardSaleLabel { get; set; }
        public string DianCardStocksLabel { get; set; }
        public string HosterLabel { get; set; }
        //public string LogoffExitLabel { get; set; }
        public string PasswordChangeLabel { get; set; }
        //public string UserCashLabel { get; set; }

        public string AccountBalanceLabel { get; set; }

        protected override void InitializeResource()
        {
            WelcomeInfo = Text.WelcomeAgentPortal;
            AgentCommissionLabel = Text.AgentCommission;
            //AgentPaymentLabel = Text.AgentPayment;
            BankAccountLabel = Text.BankAccount;
            BuyDianCardLabel = Text.BuyDianCard;
            BuyGuanHaoLabel = Text.BuyGuanHao;
            BuyMembershipLabel = Text.BuyMembership;
            DepositLabel = Text.Deposit;
            DianCardSaleLabel = Text.DianCardSale;
            DianCardStocksLabel = Text.DianCardStocks;
            HosterLabel = Text.Hoster;
            //LogoffExitLabel = Text.LogoffExit;
            PasswordChangeLabel = Text.PasswordChange;
            //UserCashLabel = Text.UserCash;
            AccountBalanceLabel = Text.AccountBalance;

            SelectLabel = Text.Select;
            DianCardNameLabel = Text.DianCardNameLabel;
            DianCardPriceLabel = Text.DianCardPriceLabel;
            DianCardCountLabel = Text.DianCardCountLabel;
            DianCardBlockLabel = Text.DianCardBlockLabel;
            DianCardHosterLabel = Text.DianCardHosterLabel;
            DianCardRecommenderLabel = Text.DianCardRecommenderLabel;
            base.InitializeResource();
        }

        private List<TitledViewModel> agentPortalCommandItems;
        public List<TitledViewModel> AgentPortalCommandItems 
        {
            get
            {
                if (agentPortalCommandItems == null)
                    agentPortalCommandItems = CreateFunctionTreeItems();
                return agentPortalCommandItems;
            }
            set
            {
                ChangeAndNotify<List<TitledViewModel>>(ref agentPortalCommandItems, value, () => AgentPortalCommandItems);
            }
        }

        private FunctionNodeViewModel selectedFunction;
        public FunctionNodeViewModel SelectedFunction
        {
            get { return selectedFunction; }
            set { ChangeAndNotify<FunctionNodeViewModel>(ref selectedFunction, value, () => SelectedFunction); }
        }

        private string welcomeInfo;
        public string WelcomeInfo
        {
            get { return welcomeInfo; }
            set { ChangeAndNotify<string>(ref welcomeInfo, value, () => WelcomeInfo); }
        }

        #region BuyDianCard Resource
        public string SelectLabel { get; set; }
        public string DianCardNameLabel { get; set; }
        public string DianCardPriceLabel { get; set; }
        public string DianCardCountLabel { get; set; }
        public string DianCardBlockLabel { get; set; }
        public string DianCardHosterLabel { get; set; }
        public string DianCardRecommenderLabel { get; set; }
        #endregion

        private List<TitledViewModel> CreateFunctionTreeItems()
        {
            List<TitledViewModel> result = new List<TitledViewModel>();
            FunctionNodeViewModel func1 = new FunctionNodeViewModel(Text.AgentCommission, this, new SecureCommand(FunctionSelected), null, true);
           
            //FunctionNodeViewModel func2 = new FunctionNodeViewModel(Text.AgentPayment, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func3 = new FunctionNodeViewModel(Text.BankAccount, this, new SecureCommand(FunctionSelected));
            SelectedFunction = func3;
            FunctionNodeViewModel func4 = new FunctionNodeViewModel(Text.BuyDianCard, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func5 = new FunctionNodeViewModel(Text.BuyGuanHao, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func6 = new FunctionNodeViewModel(Text.BuyMembership, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func7 = new FunctionNodeViewModel(Text.Deposit, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func8 = new FunctionNodeViewModel(Text.DianCardSale, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func9 = new FunctionNodeViewModel(Text.DianCardStocks, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func10 = new FunctionNodeViewModel(Text.Hoster, this, new SecureCommand(FunctionSelected));
            //FunctionNodeViewModel func11 = new FunctionNodeViewModel(Text.LogoffExit, this, new SecureCommand(FunctionSelected));
            FunctionNodeViewModel func12 = new FunctionNodeViewModel(Text.PasswordChange, this, new SecureCommand(FunctionSelected));
            //FunctionNodeViewModel func13 = new FunctionNodeViewModel(Text.UserCash, this, new SecureCommand(FunctionSelected));
            
            result.Add(func3);
            result.Add(func4);
            result.Add(func6);
            result.Add(func5);
            //result.Add(func2);
            
            result.Add(func7);
            result.Add(func9);
            result.Add(func8);
            result.Add(func1);
            result.Add(func12);
            result.Add(func10);
            //result.Add(func11);
            //result.Add(func13);
            return result;
        }

        private void FunctionSelected(SecureCommandArgs arg)
        {
            FunctionNodeViewModel newValue = arg.Content as FunctionNodeViewModel;
            if (newValue != null && newValue != SelectedFunction)
            {
                if (SelectedFunction != null)
                {
                    SelectedFunction.IsSelected = true;
                }
                SelectedFunction = newValue;
                SelectedFunction.IsSelected = true;
            }
        }

        public bool AgentLogin(string agentId, string pwd, string agentPwd)
        {
            int aId = int.Parse(agentId);
            return ApplicationVM.ChatClient.AgentLogin(Me.Id, aId, pwd, agentPwd);
        }
    }

    public class FunctionNodeViewModel : TitledViewModel
    {
        private ReadOnlyCollection<FunctionNodeViewModel> functions;
        public ReadOnlyCollection<FunctionNodeViewModel> Functions { get { return functions; } }

        private SecureCommand funcCommand;
        public SecureCommand FuncCommand { get { return funcCommand; } }

        private ViewModelBase functionVM;
        public ViewModelBase FunctionVM
        {
            get { return functionVM; }
            set { ChangeAndNotify<ViewModelBase>(ref functionVM, value, () => FunctionVM); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { ChangeAndNotify<bool>(ref isSelected, value, () => IsSelected); }
        }

        public FunctionNodeViewModel(string title)
            : this(title, null)
        { }

        public FunctionNodeViewModel(string title,  ViewModelBase vm)
            : this(title, vm, null)
        { }
        public FunctionNodeViewModel(string title,  ViewModelBase vm, SecureCommand cmd)
            : this(title, vm, cmd, null)
        {

        }
        public FunctionNodeViewModel(string title, ViewModelBase vm, SecureCommand cmd, ReadOnlyCollection<FunctionNodeViewModel> funcs)
            : this(title, vm, cmd, funcs, false)
        {
        }

        public FunctionNodeViewModel(string title, ViewModelBase vm, SecureCommand cmd, ReadOnlyCollection<FunctionNodeViewModel> funcs, bool isSelected)
        {
            Title = title;
           
            functionVM = vm;
            funcCommand = cmd;
            functions = funcs;
            IsSelected = isSelected;
        }
    }
}
