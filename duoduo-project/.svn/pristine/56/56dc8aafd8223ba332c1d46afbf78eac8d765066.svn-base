using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using YoYoStudio.Common.Notification;
using WPFSoundVisualizationLib;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow 
    {
        private bool embeded = true;
        private VideoWindowViewModel videoWindowVM;
        

        public VideoWindow(VideoWindowViewModel vm, bool isEmbedded)
        {
            DataContext = vm;
            videoWindowVM = vm;
            InitializeComponent();
            vc.MoviePath = vm.FlexPath;
            vc.FlashCallback += vc_FlashCallback;
            Loaded += VideoWindow_Loaded;
            embeded = isEmbedded;

            vc.BottomTemplate = FindResource("VideoControlBottomTemplate") as ControlTemplate;
            vc.VideoBorderStyle = FindResource("VideoControlBorderStyle") as Style;
            vc.ApplyTemplate();
        }

        //private void NAudioEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    NAudioEngine engine = NAudioEngine.Instance;
        //    switch (e.PropertyName)
        //    {
        //        case "FileTag":
        //            if (engine.FileTag != null)
        //            {
        //                TagLib.Tag tag = engine.FileTag.Tag;
        //                if (tag.Pictures.Length > 0)
        //                {
        //                    using (MemoryStream albumArtworkMemStream = new MemoryStream(tag.Pictures[0].Data.Data))
        //                    {
        //                        try
        //                        {
        //                            BitmapImage albumImage = new BitmapImage();
        //                            albumImage.BeginInit();
        //                            albumImage.CacheOption = BitmapCacheOption.OnLoad;
        //                            albumImage.StreamSource = albumArtworkMemStream;
        //                            albumImage.EndInit();
        //                            albumArtPanel.AlbumArtImage = albumImage;
        //                        }
        //                        catch (NotSupportedException)
        //                        {
        //                            albumArtPanel.AlbumArtImage = null;
        //                            // System.NotSupportedException:
        //                            // No imaging component suitable to complete this operation was found.
        //                        }
        //                        albumArtworkMemStream.Close();
        //                    }
        //                }
        //                else
        //                {
        //                    albumArtPanel.AlbumArtImage = null;
        //                }
        //            }
        //            else
        //            {
        //                albumArtPanel.AlbumArtImage = null;
        //            }
        //            break;
        //        case "ChannelPosition":
        //            clockDisplay.Time = TimeSpan.FromSeconds(engine.ChannelPosition);
        //            break;
        //        default:
        //            // Do Nothing
        //            break;
        //    }

        //}

        void VideoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!embeded)
            {
                WindowStyle = System.Windows.WindowStyle.ToolWindow;
            }

            
        }

        void vc_FlashCallback(YoYoStudio.Controls.Winform.FlexCallbackCommand cmd, List<string> args)
        {
            VideoWindowViewModel vm = DataContext as VideoWindowViewModel;
            if (vm != null && vm.UserVM != null)
            {
                switch (cmd)
                {
                    case YoYoStudio.Controls.Winform.FlexCallbackCommand.ZoomIn:
                        Width = Width + 30.0;
                        Height = Height + 30.0;
                        Topmost = true;
                        break;
                    case YoYoStudio.Controls.Winform.FlexCallbackCommand.ZoomOut:
                        Width = Width - 30.0;
                        Height = Height - 30.0;
                        Topmost = true;
                        break;
                    case YoYoStudio.Controls.Winform.FlexCallbackCommand.LoadComplete:
                        vc.CallFlash(YoYoStudio.Controls.Winform.FlexCommand.ConnectRTMP, vm.UserVM.RoomWindowVM.RoomVM.RtmpUrl);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void ProcessMessage(EnumNotificationMessage<object, VideoWindowAction> message)
        {
        }

        public override void Dispose()
        {
            vc.Dispose();
            base.Dispose();
        }
    }
}
