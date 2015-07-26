using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Resource;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for AgentLoginWindow.xaml
    /// </summary>
    public partial class AgentLoginWindow
    {
        AgentPortalWindowViewModel vm = null;
        public AgentLoginWindow(AgentPortalWindowViewModel viewModel)
            :base(viewModel)
        {
            vm = viewModel;
            InitializeComponent();
            MinHeight = ActualHeight;
            MaximizeButtonState = YoYoStudio.Controls.CustomWindow.WindowButtonState.Disabled;
        }

        protected override void ProcessMessage(Common.Notification.EnumNotificationMessage<object, AgentPortalWindowAction> message)
        {
            throw new NotImplementedException();
        }

        private void ApplyAgent(object sender, RoutedEventArgs e)
        { 
            
        }

        private void AgentLogon(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UserIdTxt.Text)
                || string.IsNullOrEmpty(PwdTxt.Text)
               )
            {
                MessageBox.Show("用户名，密码不能为空");
                return;
            }
            bool result = vm.AgentLogin(UserIdTxt.Text, PwdTxt.Text, "");
            if (result)
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(Messages.LoginFailed);
            }
        }

        
    }
}
