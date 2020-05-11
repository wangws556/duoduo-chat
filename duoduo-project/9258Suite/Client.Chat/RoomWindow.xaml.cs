﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using YoYoStudio.Client.Chat.Controls;
using YoYoStudio.Client.Chat.WebPages;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common;
using YoYoStudio.Common.Wpf;
using YoYoStudio.Model.Chat;
using YoYoStudio.Model.Core;
using YoYoStudio.Model.Json;
using YoYoStudio.Model.Media;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for RoomWindow.xaml
    /// </summary>
    public partial class RoomWindow
    {
        private RoomWindowViewModel roomWindowVM;
        private HallWindow hallWindow;
        //private PlayMusicWindow playMusicWindow = null;
        private bool hasClicked = false;
        public RoomWindow(RoomWindowViewModel roomWindowVM):this(roomWindowVM,null)
        {
        }

        public RoomWindow(RoomWindowViewModel roomWindowVM, HallWindow hwnd)
            :base(roomWindowVM)
        {
            roomWindowVM.Load(AllWebPages.RoomPage);
            roomWindowVM.Initialize();
            this.roomWindowVM = roomWindowVM;
            InitializeComponent();
            Loaded += RoomWindow_Loaded;
            //MouseLeftButtonUp += RoomWindow_MouseLeftButtonUp;
            hallWindow = hwnd;
            //if(hallWindow != null)
            //    hallWindow.StateChanged += hallWindow_StateChanged;
        }

        //void RoomWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    Topmost = false;
        //}

        private void InitMusicWindow()
        {
            //if (playMusicWindow == null)
            //{
            //    PlayMusicWindowViewModel playMusicVM = new PlayMusicWindowViewModel();
            //    playMusicWindow = new PlayMusicWindow(playMusicVM);
            //    //playMusicWindow.StateChanged += playMusicWindow_StateChanged;
            //    playMusicWindow.Closed += playMusicWindow_Closed;
            //}
        }

        //void hallWindow_StateChanged(object sender, EventArgs e)
        //{
        //    HallWindow hwnd = sender as HallWindow;
        //    if (hwnd != null)
        //    {
        //        if (hwnd.WindowState != System.Windows.WindowState.Minimized)
        //            WindowState = System.Windows.WindowState.Minimized;
        //    }
        //}

        void RoomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
            CreateVideoWindow(videoBorder1, roomWindowVM.FirstVideoWindowVM);
            CreateVideoWindow(videoBorder2, roomWindowVM.SecondVideoWindowVM);
            CreateVideoWindow(videoBorder3, roomWindowVM.ThirdVideoWindowVM);
            webWindow = CreateWebWindow() as WebWindow;
            //Task.Factory.StartNew(() =>
            //    {
            //        roomWindowVM.InitializeAudio();

            //    });
            //InitMusicWindow();
            //ConnectLiveRtmp();
        }

        private void ConnectLiveRtmp()
        {
            AudioWindowViewModel audioVM = new AudioWindowViewModel();
            AudioWindow aw = new AudioWindow(audioVM);
            aw.Show();
            aw.Visibility = Visibility.Hidden;
            aw.ShowInTaskbar = false;
        }

        private Window CreateWebWindow()
        {
            Point p = PART_Web.TransformToAncestor(this).Transform(new Point(0, 0));
            double x = p.X;
            double y = p.Y;
            YoYoStudio.Controls.CustomWindow.ChildWindow<WebWindowAction> webWindow = new WebWindow(roomWindowVM);
            //webWindow = new WebWindow(roomWindowVM);
            webWindow.SetParent<RoomWindowAction>(this, true);
            webWindow.OffsetX = x;
            webWindow.OffsetY = y;
            webWindow.ReplicatedControl = PART_Web;
            webWindow.Top = Top + webWindow.OffsetY;
            webWindow.Left = Left + webWindow.OffsetX;
            
            webWindow.Owner = this;
            webWindow.Show();
            webWindow.Topmost = false;
            return webWindow;
            //Point p = PART_Web.TransformToAncestor(this).Transform(new Point(0, 0));
            //double x = p.X;
            //double y = p.Y;
            //return CreateWindowInSeparateThread<WebWindowAction>(() =>
            //{
            //    return new WebWindow(roomWindowVM);
            //},
            //    x, y, false, true, PART_Web);
        }

        private Window CreateVideoWindow(ContentControl videoBorder,VideoWindowViewModel vm)
        {
            Point p = videoBorder.TransformToAncestor(this).Transform(new Point(0, 0));
            double x = p.X;
            double y = p.Y;
            YoYoStudio.Controls.CustomWindow.ChildWindow<VideoWindowAction> videoWnd = new VideoWindow(vm, true);
            //VideoWindow videoWnd = new VideoWindow(vm, true);
            videoWnd.SetParent<RoomWindowAction>(this, true);
            videoWnd.OffsetX = x;
            videoWnd.OffsetY = y;
            videoWnd.ReplicatedControl = videoBorder;
            videoWnd.Top = Top + videoWnd.OffsetY;
            videoWnd.Left = Left + videoWnd.OffsetX;
            
            videoWnd.Owner = this;
            videoWnd.Show();
            return videoWnd;

            //Point p = videoBorder.TransformToAncestor(this).Transform(new Point(0, 0));
            //double x = p.X;
            //double y = p.Y;
            //VideoWindow wnd = CreateWindowInSeparateThread<VideoWindowAction>(() =>
            //{
            //    return new VideoWindow(vm, true);
            //},
            //    x, y, false, true, videoBorder) as VideoWindow;

            //wnd.MouseLeftButtonUp += wnd_MouseLeftButtonUp;
            //return wnd;
        }

        //void wnd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
            
        //    //System.Windows.MessageBox.Show(e.ClickCount.ToString());
        //    Dispatcher.Invoke((Action)(() =>
        //    {
        //        Topmost = true;
        //    }));
        //    this.RaiseWindowHeaderMouseLeftButtonDownEvent(this, null);
        //}

		protected override void ProcessMessage(Common.Notification.EnumNotificationMessage<object, RoomWindowAction> message)
        {
            RoomWindowViewModel roomVM = DataContext as RoomWindowViewModel;
            switch (message.Action)
            {
                case RoomWindowAction.ShowConfigWindow:
                    ConfigurationItemViewModel configItem = message.Content as ConfigurationItemViewModel;
                    roomVM.ApplicationVM.ConfigurationWindowVM = new ConfigurationWindowViewModel(configItem);
                    //ShowWebWindow(webWindow,false);
                    ConfigurationWindow configurationWindow = new ConfigurationWindow(roomVM.ApplicationVM.ConfigurationWindowVM);
                    //configurationWindow.Owner = this;
                    configurationWindow.ShowInTaskbar = true;
                    configurationWindow.ShowDialog();
                    //ShowWebWindow(webWindow,true);
                    break;
                case RoomWindowAction.PlayMusic:
                    //bool canPlay = (bool)message.Content;
                    //if (canPlay)
                    //{
                    //    if (playMusicWindow == null)
                    //        InitMusicWindow();
                    //    //ShowWebWindow(webWindow, false);
                    //    playMusicWindow.WindowState = System.Windows.WindowState.Normal;
                    //    playMusicWindow.Show();
                    //    playMusicWindow.Topmost = true;
                    //}
                    //else
                    //{
                    //    //ShowWebWindow(webWindow, false);
                    //    System.Windows.MessageBox.Show("其它人正在播放音乐，请稍后再试", "提示", MessageBoxButton.OK);
                    //        //ShowWebWindow(webWindow, true);
                    //}
                    break;
                
                case RoomWindowAction.ManageMusic:
                    //ShowWebWindow(webWindow, false);
                    ManageMusicWindowViewModel manageMusicVM = new ManageMusicWindowViewModel();
                    ManageMusicWindow manageWnd = new ManageMusicWindow(manageMusicVM);
                    //manageWnd.Owner = this;
                    manageWnd.Topmost = true;
                    //ShowWebWindow(webWindow, false);
                    manageWnd.ShowInTaskbar = true;
                    manageWnd.ShowDialog();
                    //ShowWebWindow(webWindow, true);
                    break;

                case RoomWindowAction.RecordAudio:
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Filter = "音频文件 (*.wav)|*.wav";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        RoomWindowViewModel vm = DataContext as RoomWindowViewModel;
                        vm.StartAudioRecording(dialog.FileName);
                    }
                    break;
                case RoomWindowAction.PublishWarning:
                    System.Windows.MessageBox.Show("网络不稳定，可能导致房间收听者卡顿，建议先关闭在麦上唱歌再开始", "警告", MessageBoxButton.OK);
                    break;
                case RoomWindowAction.PublishExit:
                    System.Windows.MessageBox.Show("在麦上唱歌程序异常退出，请关闭在麦上唱歌再开始", "错误", MessageBoxButton.OK);
                    break;
                case RoomWindowAction.PlayError:
                    //System.Windows.MessageBox.Show("音乐播放程序异常退出", "错误", MessageBoxButton.OK);
                    break;
                case RoomWindowAction.PlayExit:
                    //System.Windows.MessageBox.Show("音乐播放程序报错", "错误", MessageBoxButton.OK);
                    break;
                default:
                    break;
            }
        }

        void playMusicWindow_Closed(object sender, EventArgs e)
        {
            //playMusicWindow.StateChanged -= playMusicWindow_StateChanged;
            //playMusicWindow.Closed -= playMusicWindow_Closed;
            //playMusicWindow = null;
            //ShowWebWindow(webWindow, true);
            //InitMusicWindow();
            //if ((roomWindowVM.Me.MicStatus & MicStatusMessage.MicStatus_VideoAudio) != 0)
            //{
            //    roomWindowVM.Me.DownMic();
            //}
        }

        //void playMusicWindow_StateChanged(object sender, EventArgs e)
        //{

        //    if (playMusicWindow.WindowState == System.Windows.WindowState.Minimized)
        //    {
        //        if (this.WindowState != System.Windows.WindowState.Minimized)
        //            //ShowWebWindow(webWindow, true);
        //    }
        //    else if (playMusicWindow.WindowState == System.Windows.WindowState.Normal)
        //    {
        //        if(this.WindowState != System.Windows.WindowState.Minimized)
        //            //ShowWebWindow(webWindow, false);
        //    }
        //}
        

        public override void CallJavaScript(string functionName, params object[] args)
        {
            webWindow.CallJavaScript(functionName, args);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if ((roomWindowVM.Me.MicStatus & MicStatusMessage.MicStatus_VideoAudio) != 0)
            {
                roomWindowVM.RoomClient.DownMic(roomWindowVM.RoomVM.Id, roomWindowVM.Me.MicType, roomWindowVM.Me.MicIndex);
                roomWindowVM.Me.DownMic();
            }
            roomWindowVM.AudioPlayVM.StopPlay();
            base.OnClosing(e);
        }

        //protected override void OnStandardWindowActivated(object sender, EventArgs e)
        //{
           
        //    base.OnStandardWindowActivated(sender, e);

        //    if (this.WindowState != System.Windows.WindowState.Minimized)
        //    {
        //        if (_maximizeButton.Visibility != System.Windows.Visibility.Visible)
        //            WindowState = System.Windows.WindowState.Maximized;
        //        else
        //            WindowState = System.Windows.WindowState.Normal;
        //    }
        //    else
        //    {
        //        if (_minimizeButton.Visibility == System.Windows.Visibility.Collapsed)
        //        {
        //            if (_maximizeButton.Visibility != System.Windows.Visibility.Visible)
        //                WindowState = System.Windows.WindowState.Maximized;
        //            else
        //                WindowState = System.Windows.WindowState.Normal;
        //        }
        //    }
        //    if (hasClicked)
        //        Topmost = true;
        //}

        //protected override void OnStandardWindowDeactivated(object sender, EventArgs e)
        //{
        //    base.OnStandardWindowDeactivated(sender, e);
        //    if (this.WindowState != System.Windows.WindowState.Minimized)
        //    {
        //        if (_maximizeButton.Visibility != System.Windows.Visibility.Visible)
        //            WindowState = System.Windows.WindowState.Maximized;
        //        else
        //            WindowState = System.Windows.WindowState.Normal;
        //    }
        //    Topmost = false;
        //}

        //protected override void OnStandardWindow_StateChanged(object sender, EventArgs e)
        //{
        //    base.OnStandardWindow_StateChanged(sender, e);
        //    if (this.WindowState != System.Windows.WindowState.Minimized)
        //    {
        //        if (_maximizeButton.Visibility != System.Windows.Visibility.Visible)
        //            WindowState = System.Windows.WindowState.Maximized;
        //        else
        //            WindowState = System.Windows.WindowState.Normal;
        //        Topmost = false;
        //    }
        //}

        //protected override void OnWindowHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    base.OnWindowHeaderMouseLeftButtonDown(sender, e);
        //    hasClicked = true;
        //    Topmost = false;
        //    //RaiseWindowHeaderMouseLeftButtonDownEvent(sender, e);
        //}

       
    }
}
