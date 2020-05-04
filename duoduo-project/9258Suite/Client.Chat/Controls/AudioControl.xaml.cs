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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Controls.Winform;

namespace YoYoStudio.Client.Chat.Controls
{
    /// <summary>
    /// Interaction logic for AudioControl.xaml
    /// </summary>
    public partial class AudioControl : UserControl,IDisposable
    {
        public string MoviePath
        {
            get { return (string)GetValue(MoviePathProperty); }
            set { SetValue(MoviePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoviePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoviePathProperty =
            DependencyProperty.Register("MoviePath", typeof(string), typeof(AudioControl), new PropertyMetadata(string.Empty, (o, e) =>
            {
                AudioControl vc = o as AudioControl;
                if (vc != null && e.NewValue != null)
                {
                    vc.flex.MoviePath = e.NewValue.ToString();
                    vc.flex.LoadFlex();
                }
            }));

        public event FlashCallbackEventHandler FlashCallback;

        public AudioControl()
        {
            InitializeComponent();
        }

        private void flex_FlashCallback(YoYoStudio.Controls.Winform.FlexCallbackCommand cmd, List<string> args)
        {
            AudioWindowViewModel vm = DataContext as AudioWindowViewModel;
            switch (cmd)
            {
                case FlexCallbackCommand.None:
                    break;
                case FlexCallbackCommand.ReportStatus:
                    if (args != null && args.Count == 1)
                    {
                        if (args[0] == FlexStatusStrings.ConnectSucceed)
                        {
                            RtmpConnectSuccessful();
                            //the connection has been setup with Red5
                            if (vm != null)
                            {
                                vm.RoomWindowVM.RoomCallback.MicStatusMessageReceivedEvent += AudioRoomCallback_MicStatusMessageReceivedEvent;
                            }
                        }
                    }
                    break;
                case FlexCallbackCommand.LoadComplete:
                    //flex control has been loaded. Next we will connect the Rmtp and load the musics

                    if (vm != null)
                    {
                        CallFlash(FlexCommand.ConnectRTMP, vm.AudioRtmpUrl);
                    }
                    break;
                case FlexCallbackCommand.PlayMusic:
                    if (vm.RoomWindowVM.RoomClient != null)
                        vm.RoomWindowVM.RoomClient.StartMusic(vm.RoomWindowVM.RoomVM.Id, vm.Me.Id, args[0]);
                    break;
                case FlexCallbackCommand.StopMusic:
                    if (vm.RoomWindowVM.RoomClient != null)
                        vm.RoomWindowVM.RoomClient.StopMusic(vm.RoomWindowVM.RoomVM.Id, vm.Me.Id);
                    break;
                case FlexCallbackCommand.SetPlayPosition:
                    //vm.RoomWindowVM.RoomClient.SetPlayPosition(vm.RoomWindowVM.RoomVM.Id, vm.Me.Id, int.Parse(args[0]));
                    break;
                case FlexCallbackCommand.SetVolume:
                    //vm.RoomWindowVM.RoomClient.SetMusicVolume(vm.RoomWindowVM.RoomVM.Id, vm.Me.Id, int.Parse(args[0]));
                    break;
                case FlexCallbackCommand.TogglePauseMusic:
                    //if (args != null && args.Count == 1)
                    //{
                    //    if (args[0] == FlexStatusStrings.MusicPaused)
                    //    {
                    //        vm.RoomWindowVM.RoomClient.TogglePauseMusic(vm.RoomWindowVM.RoomVM.Id, vm.Me.Id, true);
                    //    }
                    //    else
                    //    {
                    //        vm.RoomWindowVM.RoomClient.TogglePauseMusic(vm.RoomWindowVM.RoomVM.Id, vm.Me.Id,false);
                    //    }
                    //}
                    break;
                default:
                    break;
            }

            if (FlashCallback != null)
            {
                FlashCallback(cmd, args);
            }
        }

        private void RtmpConnectSuccessful()
        {
            AudioWindowViewModel vm = DataContext as AudioWindowViewModel;
            CallFlash(FlexCommand.Connect,
                vm.RoomWindowVM.RoomVM.RoomAudioStreamId);
        }

        public string[] CallFlash(FlexCommand cmd, params string[] args)
        {
            return flex.CallFlash(cmd, args);
        }

        private void AudioRoomCallback_MicStatusMessageReceivedEvent(int arg1, Model.Chat.MicStatusMessage arg2)
        {
            AudioWindowViewModel vm = DataContext as AudioWindowViewModel;
            if (vm != null)
            {
                if (arg1 == vm.RoomWindowVM.RoomVM.Id)
                {
                    var uvm = vm.RoomWindowVM.UserVMs.FirstOrDefault(u => u.Id == arg2.UserId);
                    if(uvm != null)
                    {
                        switch (arg2.MicAction)
                        {
                            case Model.Chat.MicAction.None:
                                break;
                            case Model.Chat.MicAction.OnMic:
                                switch (arg2.MicType)
                                {
                                    case Model.Chat.MicType.None:
                                        break;
                                    case Model.Chat.MicType.Public:
                                        CallFlash(FlexCommand.PlayMusic);
                                        break;
                                    case Model.Chat.MicType.Private:
                                        break;
                                    case Model.Chat.MicType.Secret:
                                        break;
                                    case Model.Chat.MicType.Max:
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case Model.Chat.MicAction.Toggle:
                                break;
                            case Model.Chat.MicAction.DownMic:
                                switch (arg2.MicType)
                                {
                                    case Model.Chat.MicType.None:
                                        break;
                                    case Model.Chat.MicType.Public:
                                        CallFlash(FlexCommand.StopMusic);
                                        break;
                                    case Model.Chat.MicType.Private:
                                        break;
                                    case Model.Chat.MicType.Secret:
                                        break;
                                    case Model.Chat.MicType.Max:
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {

            }));
            flex.Dispose();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
