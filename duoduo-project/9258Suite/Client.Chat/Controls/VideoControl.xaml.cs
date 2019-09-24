﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFSoundVisualizationLib;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common;
using YoYoStudio.Controls;
using YoYoStudio.Controls.Winform;
using YoYoStudio.Model.Chat;

namespace YoYoStudio.Client.Chat.Controls
{
    /// <summary>
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    [TemplatePart(Name="PART_Bottom", Type=typeof(ContentControl))]
    public partial class VideoControl : UserControl, IDisposable
    {
        #region Properties

        public Visibility BottomVisibility
        {
            get { return (Visibility)GetValue(BottomVisibilityProperty); }
            set { SetValue(BottomVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomVisibilityProperty =
            DependencyProperty.Register("BottomVisibility", typeof(Visibility), typeof(VideoControl), new PropertyMetadata(Visibility.Hidden, (o, e) =>
                {
                    VideoControl vc = o as VideoControl;
                    if (e.NewValue != null && vc != null)
                    {
                        Visibility v = (Visibility)e.NewValue;
                        vc.PART_Bottom.Visibility = v;
                        if (v == Visibility.Visible)
                        {
                            vc.PART_Bottom.Height = double.NaN;
                        }
                        else
                        {
                            vc.PART_Bottom.Height = 0;
                        }
                    }
                }));



        public Style VideoBorderStyle
        {
            get { return (Style)GetValue(VideoBorderStyleProperty); }
            set { SetValue(VideoBorderStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoBorderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoBorderStyleProperty =
            DependencyProperty.Register("VideoBorderStyle", typeof(Style), typeof(VideoControl), new PropertyMetadata(null, (o, e) =>
                {
                    VideoControl vc = o as VideoControl;
                    if (e.NewValue != null && vc != null)
                    {
                        Style style = e.NewValue as Style;
                        vc.PART_VideoBorder.Style = style;
                    }
                }));

		public ControlTemplate BottomTemplate
		{
            get { return (ControlTemplate)GetValue(BottomTemplateProperty); }
            set { SetValue(BottomTemplateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BottomTemplate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BottomTemplateProperty =
			DependencyProperty.Register("BottomTemplate", typeof(ControlTemplate), typeof(VideoControl), new PropertyMetadata(null));


        public int MicStatus
        {
            get { return (int)GetValue(MicStatusProperty); }
            set { SetValue(MicStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MicStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MicStatusProperty =
            DependencyProperty.Register("MicStatus", typeof(int), typeof(VideoControl), new PropertyMetadata(0,
                (o, e) =>
                {
                    VideoControl vc = o as VideoControl;
                    if (vc != null)
                    {
                        vc.MicStatusChangedHander(e.OldValue, e.NewValue);
                    }
                }));



        public string MoviePath
        {
            get { return (string)GetValue(MoviePathProperty); }
            set { SetValue(MoviePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoviePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoviePathProperty =
            DependencyProperty.Register("MoviePath", typeof(string), typeof(VideoControl), new PropertyMetadata(string.Empty, (o, e) =>
                {
                    VideoControl vc = o as VideoControl;
                    if (vc != null && e.NewValue != null)
                    {
                        vc.flex.MoviePath = e.NewValue.ToString();
                        vc.flex.LoadFlex();
                    }
                }));



        public WebPage RoomPage
        {
            get { return (WebPage)GetValue(RoomPageProperty); }
            set { SetValue(RoomPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoomPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoomPageProperty =
            DependencyProperty.Register("RoomPage", typeof(WebPage), typeof(VideoControl), new PropertyMetadata(null));

        public bool IsZoom
        {
            get { return (bool)GetValue(IsZoomProperty); }
            set { SetValue(IsZoomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZoomProperty =
            DependencyProperty.Register("IsZoom", typeof(bool), typeof(VideoControl), new PropertyMetadata(false));

        #endregion

        private RealTimePlayback playBack;
        private SpectrumAnalyzer spectrumAnalyzer;
        public event FlashCallbackEventHandler FlashCallback;
        public bool HasZoomed { get; set; }

        public VideoControl()
        {
            InitializeComponent();
            IsEnabled = false;
            IsZoom = false;
        }

        public void Dispose()
        {
            Dispatcher.BeginInvoke((Action)(() =>
                {
                    UserViewModel uvm = DataContext as UserViewModel;
                    if (uvm != null)
                    {
                        RoomWindowViewModel roomWindowVM = uvm.RoomWindowVM;
                        if (roomWindowVM != null)
                        {
                            if (roomWindowVM.RoomCallback != null)
                            {
                                roomWindowVM.RoomCallback.VideoStateChangedEvent -= VideoStateChangedEventHandler;
                                roomWindowVM.RoomCallback.AudioStateChangedEvent -= AudioStateChangedEventHandler;
                            }
                        }
                    }
                    flex.Dispose();
                }));
        }

        public string[] CallFlash(FlexCommand cmd, params string[] args)
        {
            return flex.CallFlash(cmd, args);
        }

        #region Private

        private void RtmpConnectSuccessful()
        {
            UserViewModel uvm = DataContext as UserViewModel;
            if (uvm != null)
            {
                int idx = 0;
                int micIdx = 0;
                if (uvm.IsMe())
                {
                    idx = uvm.ApplicationVM.ProfileVM.VideoConfigurationVM.CameraIndex;
                }
                bool pv = (uvm.MicStatus & MicStatusMessage.MicStatus_Video) != MicStatusMessage.MicStatus_Off;
                bool pa = (uvm.MicStatus & MicStatusMessage.MicStatus_Audio) != MicStatusMessage.MicStatus_Off;

                CallFlash(FlexCommand.Connect,
                    uvm.StreamGuid,
                    uvm.IsMe() ? "1" : "0",
                    idx.ToString(),
                    micIdx.ToString(),
                    pv ? "1" : "0",
                    pa ? "1" : "0",
                    IsZoom ? "1" : "0",
                    uvm.ApplicationVM.LocalCache.VideoFps.ToString(),
                    uvm.ApplicationVM.LocalCache.VideoQuality.ToString());
                RoomWindowViewModel roomWindowVM = uvm.RoomWindowVM;
                roomWindowVM.RoomCallback.VideoStateChangedEvent += VideoStateChangedEventHandler;
                roomWindowVM.RoomCallback.AudioStateChangedEvent += AudioStateChangedEventHandler;
            }
        }

        private void flex_FlashCallback(FlexCallbackCommand cmd, List<string> args)
        {
            UserViewModel uvm = DataContext as UserViewModel;
            if (uvm != null && uvm.RoomWindowVM != null)
                switch (cmd)
                {
                    case FlexCallbackCommand.ReportStatus:
                        IsEnabled = true;
                        if (args != null && args.Count == 1)
                        {
                            if (args[0] == FlexStatusStrings.ConnectSucceed)
                            {
                                RtmpConnectSuccessful();
                            }
                            else
                            {
                                //TODO Connect Red5 failed.
                            }
                        }
                        break;
                    case FlexCallbackCommand.LoadComplete:
                        IsEnabled = true;
                        break;
                    case FlexCallbackCommand.VideoStateChanged:
                        int vState = 0;
                        if (uvm.IsMe() && args.Count > 0)
                        {
                            if (int.TryParse(args[0], out vState))
                            {
                                uvm.RoomWindowVM.RoomClient.VideoStateChanged(uvm.RoomWindowVM.RoomVM.Id, vState);
                            }
                        }
                        break;
                    case FlexCallbackCommand.AudioStateChanged:
                        int aState = 0;
                        if (args.Count > 0)
                        {
                            if (int.TryParse(args[0], out aState))
                            {
                                uvm.RoomWindowVM.RoomClient.AudioStateChanged(uvm.RoomWindowVM.RoomVM.Id,aState);
                                //SpectrumAnalyzer spectrumAnalyzer = UIHelper.FindChild<SpectrumAnalyzer>(this, "mySpectrumAnalyzer");
                                //switch (aState)
                                //{
                                //    case FlexCallbackCommandNames.AV_State_Normal:
                                //        if (uvm.IsMe())
                                //        {
                                //            //uvm.RoomWindowVM.StartAudioRecording();
                                //            uvm.RoomWindowVM.StartAudioPublish(uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioDeviceName,uvm.RoomWindowVM.ApplicationVM.ProfileVM.AudioConfigurationVM.AudioRTMP);
                                //        }
                                //        else
                                //        {
                                //            //uvm.RoomWindowVM.StartAudioPlaying(uvm.Id);
                                //            uvm.RoomWindowVM.StartAudioPlay(uvm.RoomWindowVM.ApplicationVM.ProfileVM.AudioConfigurationVM.AudioRTMP);
                                //        }
                                //         playBack.Start();
                                //        spectrumAnalyzer.Visibility = System.Windows.Visibility.Visible;
                                //        break;
                                //    case FlexCallbackCommandNames.AV_State_Paused:
                                //        if (uvm.IsMe())
                                //        {
                                //            //uvm.RoomWindowVM.PauseAudioRecording();
                                //            uvm.RoomWindowVM.StopAudioPublish();
                                //        }
                                //        else
                                //        {
                                //            //uvm.RoomWindowVM.StopAudioPlaying(uvm.Id);
                                //            uvm.RoomWindowVM.StopAudioPlay();
                                //        }
                                //        playBack.Stop();
                                //        spectrumAnalyzer.Visibility = System.Windows.Visibility.Hidden;
                                //        break;
                                //    case FlexCallbackCommandNames.AV_State_Resumed:
                                //        if (uvm.IsMe())
                                //        {
                                //            //uvm.RoomWindowVM.StartAudioRecording();
                                //            uvm.RoomWindowVM.StartAudioPublish(uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioDeviceName, uvm.RoomWindowVM.ApplicationVM.ProfileVM.AudioConfigurationVM.AudioRTMP);
                                //        }
                                //        else
                                //        {
                                //            //uvm.RoomWindowVM.StartAudioPlaying(uvm.Id);
                                //            uvm.RoomWindowVM.StartAudioPlay(uvm.RoomWindowVM.ApplicationVM.ProfileVM.AudioConfigurationVM.AudioRTMP);
                                //        }
                                //        playBack.Start();
                                //        spectrumAnalyzer.Visibility = System.Windows.Visibility.Visible;
                                //        break;
                                //    default:
                                //        break;
                                //}
                            }
                        }

                        break;
                    case FlexCallbackCommand.TakePicture:                        
                        break;
                    case FlexCallbackCommand.ExtendVideo:
                        if (!HasZoomed)
                        {
                            VideoWindowViewModel vm = new VideoWindowViewModel(uvm);
                            vm.Initialize();
                            VideoWindow window = new VideoWindow(vm, false, this);
                            window.Topmost = true;
                            window.Show();
                            CallFlash(FlexCommand.PauseVideo);
                            HasZoomed = true;
                        }
                        break;
                }
            if (FlashCallback != null)
            {
                FlashCallback(cmd, args);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldUvm = e.OldValue as UserViewModel;
            var newUvm = e.NewValue as UserViewModel;
            if (newUvm == null && oldUvm != null)
            {
                CallFlash(FlexCommand.Disconnect);
            }
            PART_Bottom.IsEnabled = e.NewValue != null;
        }

        private void MicStatusChangedHander(object oldStatus, object newStatus)
        {
            int oldS = (int)oldStatus;
            int newS = (int)newStatus;
            UserViewModel uvm = DataContext as UserViewModel;
            if (oldS != newS)
            {
                if (newS == MicStatusMessage.MicStatus_Off)
                {
                    if (oldS != MicStatusMessage.MicStatus_Off)
                    {
                        Utility.StopPublishAudio();
                        //playBack.Stop();
                        //spectrumAnalyzer.Visibility = System.Windows.Visibility.Hidden;
                        if (uvm != null)
                            DataContext = null;
                    }
                }
                
                if (uvm != null && newS != MicStatusMessage.MicStatus_Off)
                {
                    if (oldS == MicStatusMessage.MicStatus_Off)
                    {
                        CallFlash(FlexCommand.ConnectRTMP, uvm.RoomWindowVM.RoomVM.RtmpUrl);
                    }
                    else
                    {
                        if ((newS & MicStatusMessage.MicStatus_Video) != MicStatusMessage.MicStatus_Off)
                        {
                            if ((oldS & MicStatusMessage.MicStatus_Video) == MicStatusMessage.MicStatus_Off)
                            {
                                if (uvm.MicAction == MicAction.OnMic)
                                {
                                    CallFlash(FlexCommand.PublishVideo, uvm.ApplicationVM.ProfileVM.VideoConfigurationVM.CameraIndex.ToString(),
                                        uvm.ApplicationVM.LocalCache.VideoFps.ToString(), uvm.ApplicationVM.LocalCache.VideoQuality.ToString());
                                }
                                else if (uvm.MicAction == MicAction.Toggle)
                                {
                                    CallFlash(FlexCommand.ResumeVideo);
                                }
                            }
                        }
                        if ((newS & MicStatusMessage.MicStatus_Audio) != MicStatusMessage.MicStatus_Off)
                        {
                            if ((oldS & MicStatusMessage.MicStatus_Audio) == MicStatusMessage.MicStatus_Off)
                            {
                                if (uvm.MicAction == MicAction.OnMic)
                                {
                                    CallFlash(FlexCommand.PublishAudio);
                                }
                                else if (uvm.MicAction == MicAction.Toggle)
                                {
                                    CallFlash(FlexCommand.ResumeAudio);
                                }
                            }
                        }
                    }
                }
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (spectrumAnalyzer == null)
            {
                spectrumAnalyzer = UIHelper.FindChild<SpectrumAnalyzer>(this.PART_Bottom, "mySpectrumAnalyzer");
            }
            if (playBack == null)
            {
                playBack = new RealTimePlayback();
                playBack.Start();
            }
            
            if(spectrumAnalyzer != null)
                spectrumAnalyzer.RegisterSoundPlayer(playBack);
            
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (BottomTemplate != null)
            {
                PART_Bottom.Template = BottomTemplate;
            }
            base.OnApplyTemplate();
        }

        private void AudioStateChangedEventHandler(int roomId, int senderId, int state)
        {
            Dispatcher.BeginInvoke((Action)(() =>
                {
                    UserViewModel uvm = DataContext as UserViewModel;
                    if (uvm != null && !uvm.IsMe())
                    {
                        if (roomId == uvm.RoomWindowVM.RoomVM.Id && uvm.Id == senderId)
                        {
                            if (uvm.MicStatus != MicStatusMessage.MicStatus_Off)
                            {
                                switch (state)
                                {
                                    case FlexCallbackCommandNames.AV_State_Normal:
                                        //CallFlash(FlexCommand.PublishAudio);
                                        //uvm.RoomWindowVM.StartAudioPublish(uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioDeviceName, uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioRtmpPath);
                                        //uvm.RoomWindowVM.StartAudioPlaying(uvm.Id);
                                        //uvm.RoomWindowVM.StartAudioPlay(uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioRtmpPath + "/" + roomId);

                                        break;
                                    case FlexCallbackCommandNames.AV_State_Paused:
                                        CallFlash(FlexCommand.PauseAudio);
                                        //uvm.RoomWindowVM.StopAudioPublish();
                                        //uvm.RoomWindowVM.StopAudioPlaying(uvm.Id);
                                        //uvm.RoomWindowVM.StopAudioPlay();
                                        
                                        break;
                                    case FlexCallbackCommandNames.AV_State_Resumed:
                                        CallFlash(FlexCommand.ResumeAudio);
                                        //uvm.RoomWindowVM.StartAudioPublish(uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioDeviceName, uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioRtmpPath);
                                        //uvm.RoomWindowVM.StartAudioPlaying(uvm.Id);
                                        //uvm.RoomWindowVM.StartAudioPlay(uvm.RoomWindowVM.ApplicationVM.LocalCache.AudioRtmpPath+"/"+roomId);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }));
        }

        private void VideoStateChangedEventHandler(int roomId, int senderId, int state)
        {
            Dispatcher.BeginInvoke((Action)(() =>
                {
                    UserViewModel uvm = DataContext as UserViewModel;
                    if (uvm != null && !uvm.IsMe())
                    {
                        if (roomId == uvm.RoomWindowVM.RoomVM.Id && uvm.Id == senderId)
                        {
                            if (uvm.MicStatus != MicStatusMessage.MicStatus_Off)
                            {
                                switch (state)
                                {
                                    case FlexCallbackCommandNames.AV_State_Normal:
                                        CallFlash(FlexCommand.PublishVideo, uvm.ApplicationVM.ProfileVM.VideoConfigurationVM.CameraIndex.ToString(),
                                                uvm.ApplicationVM.LocalCache.VideoFps.ToString(), uvm.ApplicationVM.LocalCache.VideoQuality.ToString());
                                        break;
                                    case FlexCallbackCommandNames.AV_State_Paused:
                                        CallFlash(FlexCommand.PauseVideo);
                                        break;
                                    case FlexCallbackCommandNames.AV_State_Resumed:
                                        CallFlash(FlexCommand.ResumeVideo);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }));
        }
    }
}
