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

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for AgentPortalWindow.xaml
    /// </summary>
    public partial class AgentPortalWindow
    {
        public AgentPortalWindow(AgentPortalWindowViewModel viewModel)
            :base(viewModel)
        {
            InitializeComponent();
            MinHeight = ActualHeight;//overide the MiniHeight set by window base
            MaximizeButtonState = YoYoStudio.Controls.CustomWindow.WindowButtonState.Disabled;
        }

        protected override void ProcessMessage(Common.Notification.EnumNotificationMessage<object, AgentPortalWindowAction> message)
        {
            switch (message.Action)
            {
                default:
                    break;
            }
        }
    }
}
