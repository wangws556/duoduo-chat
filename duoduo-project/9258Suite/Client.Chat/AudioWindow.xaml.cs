using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common.Notification;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for AudioWindow.xaml
    /// </summary>
    public partial class AudioWindow
    {
        AudioWindowViewModel audioVM = null;
        public AudioWindow(AudioWindowViewModel vm):base(vm)
        {
            if(vm != null)
            {
                audioVM = vm;
                audioVM.Initialize();
            }
            DataContext = audioVM;
            InitializeComponent();
            ac.MoviePath = audioVM.MusicFlexPath;
            ac.FlashCallback += audioControl_FlashCallback;
        }

        void audioControl_FlashCallback(YoYoStudio.Controls.Winform.FlexCallbackCommand cmd, List<string> args)
        {
            switch (cmd)
            {
                case YoYoStudio.Controls.Winform.FlexCallbackCommand.None:
                    break;
                case YoYoStudio.Controls.Winform.FlexCallbackCommand.ReportStatus:
                    break;
                case YoYoStudio.Controls.Winform.FlexCallbackCommand.LoadComplete:
                    break;
                default:
                    break;
            }

        }

        protected override void ProcessMessage(EnumNotificationMessage<object, AudioWindowAction> message)
        {
            switch (message.Action)
            {
                default:
                    break;
            }
        }
    }
}
