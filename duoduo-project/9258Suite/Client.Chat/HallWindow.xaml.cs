using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoYoStudio.Client.Chat.WebPages;
using YoYoStudio.Controls.CustomWindow;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common;
using YoYoStudio.Resource;
using YoYoStudio.Common.Wpf;
using YoYoStudio.Model.Json;
using YoYoStudio.Model.Core;
using YoYoStudio.Client.Chat.Controls;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for HallWindow.xaml
    /// </summary>
    public partial class HallWindow
    {
        private RoomWindow roomWindow;
		private HallWindowViewModel hallVM;
        public HallWindow()
            : base(Singleton<ApplicationViewModel>.Instance.HallWindowVM)
        {
            ShowInTaskbar = true;
            try
            {
                hallVM = Singleton<ApplicationViewModel>.Instance.HallWindowVM;
                hallVM.Load(AllWebPages.HallPage);
                hallVM.Initialize();
                InitializeComponent();
                Loaded += EssentialWindow_Loaded;
                Closing += HallWindow_Closing;
            }
            catch (Exception ex)
            {
                Helper.Logger.Error("Constructor of HallWindow", ex);
            }
            Helper.MainWindow = this;
        }

        void HallWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (roomWindow != null)
            {
                roomWindow.Close();
            }
        }

        protected override void ProcessMessage(Common.Notification.EnumNotificationMessage<object, ViewModel.HallWindowAction> message)
        {
            try
            {
                HallWindowViewModel hallWindowVM = DataContext as HallWindowViewModel;
                switch (message.Action)
                {
                    case HallWindowAction.ApplicationShutdown:
                        if (webWindow != null)
                        {
                            CloseWebWindow(webWindow);
                            webWindow = null;
                        }
                        if (roomWindow != null)
                        {
                            roomWindow.Close();
                        }
                        Close();
                        break;
                    case HallWindowAction.CloseRoomWindow:
                        if (roomWindow != null)
                        {
                            roomWindow.Close();
                        }
                        break;
                    case HallWindowAction.AlreadyInRoom:
                        if (MessageBox.Show(Messages.AlreadyInRoom, Text.Warning, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (roomWindow != null)
                            {
                                roomWindow.Close();
                                hallVM.Me.RoomWindowVM = null;
                            }
                            int roomId = (int)message.Content;
                            if (roomId != null)
                            {
                                hallVM.EnterRoom(roomId);
                            }
                        }
                        break;
                    case HallWindowAction.EnterRoomSucceeded:
                        RoomWindowViewModel roomWindowVM = message.Content as RoomWindowViewModel;
                        roomWindow = new RoomWindow(roomWindowVM, this) as RoomWindow;
                        roomWindow.Closed += roomWindow_Closed;
                        //roomWindow.StateChanged += roomWindow_StateChanged;
                        roomWindow.Show();
                        //Hide();
                        //ShowWebWindow(webWindow, false);
                        break;
                    case HallWindowAction.EnterRoomFailed:
                        hallVM.ApplicationVM.RoomWindowVM = null;
                        MessageBox.Show(Messages.EnterRoomFailed);
                        break;
                    //case HallWindowAction.Register:
                    //    RegisterWindowViewModel vm = message.Content as RegisterWindowViewModel;
                    //    if (vm != null)
                    //    {
                    //        registerWindow = new RegisterWindow(vm) { Owner = this };
                    //        ShowWebWindow(webWindow, false);
                    //        registerWindow.ShowDialog();
                    //        ShowWebWindow(webWindow, true);
                    //    }
                    //    break;
                    //case HallWindowAction.RegisterUserIdNotAvailable:
                    //    ShowWebWindow(webWindow, false);
                    //    MessageBox.Show(Messages.NoRegisterUserIdAvailable);
                    //    ShowWebWindow(webWindow, true);
                    //    break;
                    //case HallWindowAction.RegisterSuccess:
                    //    if (registerWindow != null)
                    //    {
                    //        registerWindow.Close();
                    //    }
                    //    User user = message.Content as User;
                    //    if (user != null)
                    //    {
                    //        webWindow.CallJavaScript("SetLoginUser", user.Id, user.Password);
                    //    }
                    //    ShowWebWindow(webWindow, true);
                    //    break;
                    //case HallWindowAction.RegisterCancel:
                    //    if (registerWindow != null)
                    //    {
                    //        registerWindow.Close();
                    //    }
                    //    ShowWebWindow(webWindow, true);
                    //    break;
                    case HallWindowAction.OpenConfigurationWindow:
                        hallWindowVM.ApplicationVM.ConfigurationWindowVM = new ConfigurationWindowViewModel();
                        ConfigurationWindow configurationWindow = new ConfigurationWindow(hallWindowVM.ApplicationVM.ConfigurationWindowVM);
                        configurationWindow.Owner = this;
                        configurationWindow.ShowDialog();
                        break;
                    case HallWindowAction.SwitchUser:
                        if (MessageBox.Show(Text.SwitchUserConfirm, Text.Prompt, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            hallWindowVM.ApplicationVM.SwitchUser();
                            LoginWindow loginWindow = new LoginWindow();
                            loginWindow.Show();
                            this.Close();
                        }
                        break;
                    case HallWindowAction.OpenAgentPortal:
                        AgentPortalWindowViewModel avm = new AgentPortalWindowViewModel();
                        avm.Initialize();
                        AgentLoginWindow agentLoginWindow = new AgentLoginWindow(avm);
                        agentLoginWindow.ShowInTaskbar = true;
                        if (agentLoginWindow.ShowDialog() == true)
                        {
                            AgentPortalWindow agentPortalWindow = new AgentPortalWindow(avm);
                            agentPortalWindow.ShowInTaskbar = true;
                            agentPortalWindow.ShowDialog();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                hallVM.ApplicationVM.Logger.Error(nameof(ProcessMessage) + " Message: " + message.Action, ex);
            }
        }

        //void roomWindow_StateChanged(object sender, EventArgs e)
        //{
        //    RoomWindow wnd = sender as RoomWindow;
        //    if (wnd != null)
        //    {
        //        if (wnd.WindowState != System.Windows.WindowState.Minimized)
        //            WindowState = System.Windows.WindowState.Minimized;
        //    }
        //}


        void roomWindow_Closed(object sender, EventArgs e)
        {
            if (roomWindow != null)
            {
                roomWindow.Closed -= roomWindow_Closed;
                //roomWindow.StateChanged -= roomWindow_StateChanged;
            }
        }

        private void EssentialWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Point p = PART_Web.TransformToAncestor(this).Transform(new Point(0, 0));
                double x = p.X;
                double y = p.Y;
                webWindow = new WebWindow(hallVM);
                webWindow.SetParent<HallWindowAction>(this, true);
                webWindow.OffsetX = x;
                webWindow.OffsetY = y;
                webWindow.ReplicatedControl = PART_Web;
                webWindow.Top = Top + webWindow.OffsetY;
                webWindow.Left = Left + webWindow.OffsetX;
                webWindow.Owner = this;
                webWindow.Topmost = false;
                webWindow.Show();
            }
            catch (Exception ex)
            {
                Helper.Logger.Error("EssentialWindow_Loaded", ex);
            }

            //Point p = PART_Web.TransformToAncestor(this).Transform(new Point(0, 0));
            //double x = p.X;
            //double y = p.Y;
            //webWindow = CreateWindowInSeparateThread<WebWindowAction>(() =>
            //{
            //    return new WebWindow(hallVM);
            //},
            //    x, y, false, true, PART_Web) as WebWindow;
        }


        public override void CallJavaScript(string functionName, params object[] args)
        {
            if (webWindow != null)
            {
                webWindow.CallJavaScript(functionName, args);
            }
        }
        public override void Dispose()
        {
            //webPage.Dispose();
            base.Dispose();
        }
    }
}
