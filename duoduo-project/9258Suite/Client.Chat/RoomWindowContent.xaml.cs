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
using YoYoStudio.Controls.CustomWindow;
using YoYoStudio.Client.Chat.Controls;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for RoomWindowContent.xaml
    /// </summary>
    public partial class RoomWindowContent 
    {
        RoomWindowViewModel roomWindowVM;
        WebWindow webWnd;

        public RoomWindowContent(RoomWindowViewModel vm)
        {
            InitializeComponent();
            roomWindowVM = vm;
            DataContext = roomWindowVM;
            Loaded += RoomWindowContent_Loaded;
        }

        void RoomWindowContent_Loaded(object sender, RoutedEventArgs e)
        {
            CreateVideoWindow(videoBorder1, roomWindowVM.FirstVideoWindowVM);
            CreateVideoWindow(videoBorder2, roomWindowVM.SecondVideoWindowVM);
            CreateVideoWindow(videoBorder3, roomWindowVM.ThirdVideoWindowVM);

            CreateWebWindow();
        }

        protected override void ProcessMessage(Common.Notification.EnumNotificationMessage<object, RoomWindowAction> message)
        {
            switch (message.Action)
            {
                default:
                    break;
            }
        }

        private void CreateWebWindow()
        {
            Point p = PART_Web.TransformToAncestor(this).Transform(new Point(0, 0));
            double x = p.X;
            double y = p.Y;
            webWnd = new WebWindow(roomWindowVM);
            webWnd.OffsetX = x;
            webWnd.OffsetY = y;
            webWnd.ReplicatedControl = PART_Web;
            webWnd.Top = Top + webWnd.OffsetY;
            webWnd.Left = Left + webWnd.OffsetX;
            webWnd.Owner = this;
            webWnd.Show();
            webWnd.Topmost = false;
            webWnd.Show();
            //Point p = PART_Web.TransformToAncestor(this).Transform(new Point(0, 0));
            //double x = p.X;
            //double y = p.Y;
            //return CreateWindowInSeparateThread<WebWindowAction>(() =>
            //{
            //    return new WebWindow(roomWindowVM);
            //},
            //    x, y, false, true, PART_Web);
        }

        private Window CreateVideoWindow(ContentControl videoBorder, VideoWindowViewModel vm)
        {
            Point p = videoBorder.TransformToAncestor(this).Transform(new Point(0, 0));
            double x = p.X;
            double y = p.Y;
            VideoWindow videoWnd = new VideoWindow(vm, true);
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
            //return CreateWindowInSeparateThread<VideoWindowAction>(() =>
            //{
            //    return new VideoWindow(vm, true);
            //},
            //    x, y, false, true, videoBorder);
        }

    }
}
