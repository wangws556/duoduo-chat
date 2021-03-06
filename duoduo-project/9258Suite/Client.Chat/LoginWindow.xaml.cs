﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using YoYoStudio.Common;
using YoYoStudio.Model.Core;
using YoYoStudio.Resource;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.ServiceModel;

namespace YoYoStudio.Client.Chat
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private HallWindowViewModel hallVM;
        private RegisterWindow registerWnd;
        private string DefaultHeaderIcon = System.IO.Path.Combine(Environment.CurrentDirectory, @"Images\Logo128.png");
        private User currentUser;
        private static String updaterModulePath;
        private static string serverVersion;
        private static string localVersion;
        private static bool needsUpgrade = false;

        public LoginWindow()
            : base(Singleton<ApplicationViewModel>.Instance.HallWindowVM)
        {
            
            ShowInTaskbar = true;
            InitializeComponent();
            MinHeight = ActualHeight;
            if(!Utility.CheckNetwork())
            {
                MessageBox.Show("没有网络连接，请稍后再试。");
                this.Close();
            }
            // Compute the updater.exe path relative to the application main module path
            updaterModulePath = System.IO.Path.Combine(Environment.CurrentDirectory, "updater.exe");
            hallVM = Singleton<ApplicationViewModel>.Instance.HallWindowVM;
            hallVM.BusyMessage = "正在检查可用更新，请稍后。。。";
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += work_DoWork;
            work.RunWorkerCompleted += work_RunWorkerCompleted;
            work.RunWorkerAsync();
            localVersion = getLocalVersion();
        }

        void work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (needsUpgrade)
            {
                MessageBox.Show("发现新版本，需要退出程序,请注意任务栏里面的升级进度。","提示",MessageBoxButton.OK);
                StartSilent();
                Environment.Exit(0);
            }

            else
            {
                hallVM.ApplicationVM.ProfileVM.LoadLoginInfo();
                if (hallVM.ApplicationVM.ProfileVM.LastLoginVM == null)
                {
                    hallVM.ApplicationVM.ProfileVM.LastLoginVM = new LoginEntryViewModel(new Model.Configuration.LoginEntry());
                }
                DataContext = hallVM.ApplicationVM.ProfileVM;
                
                MaximizeButtonState = YoYoStudio.Controls.CustomWindow.WindowButtonState.None;

                AccountListBox.SelectionChanged += AccountListBox_SelectionChanged;

                PART_CheckUpdate.Visibility = System.Windows.Visibility.Collapsed;
                PART_Login.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void work_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckUpdate();
        }

        private string getLocalVersion()
        {
            RegistryKey key;
            string result = string.Empty;
            if (Utility.Is64System())
            {
                key = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432node\YoYoStudio\9258多人视频聊天室客户端");
            }
            else
            {
                key = Registry.LocalMachine.OpenSubKey(@"Software\YoYoStudio\9258多人视频聊天室客户端");
            }
            if (key != null)
            {
                object val = key.GetValue("Version");
                if (val != null)
                    result = val.ToString();
            }
            return result;
        }

        private void CheckUpdate()
        {
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(new Uri("http://222.189.228.201/Portal/Downloads/9258Update.txt"), System.IO.Path.Combine(Environment.CurrentDirectory, "9258Update.txt"));
                //wc.DownloadFile(new Uri("http://www.9258.tv/Downloads/9258Update.txt"), System.IO.Path.Combine(Environment.CurrentDirectory, "9258Update.txt"));
            }
            catch (WebException we)
            {
                LogHelper.ErrorLogger.Error("9258Update.txt doens't exist, " + we);
                return;
            }
            List<string> texts = File.ReadLines(System.IO.Path.Combine(Environment.CurrentDirectory, "9258Update.txt")).ToList();
            if (texts != null && texts.Count > 0)
            {
                foreach (var text in texts)
                {
                    if (text.StartsWith("Version="))
                    {
                        int start = text.IndexOf('=');
                        int end = text.LastIndexOf('.');
                        serverVersion = text.Substring(start + 1,end-start-1);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(localVersion))
                {
                    System.Threading.Thread.Sleep(2000);
                }
                if (!string.IsNullOrEmpty(localVersion))
                {
                    if (string.Compare(serverVersion, localVersion) > 0)
                    {
                        needsUpgrade = true;
                    }
                }
            }
        }

        private void FileClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /**
         * This handler should be associated with a menu item that launches the
         * updater's configuration dialog.
         */
        private void OptionsAutoUpdaters_Click(object sender, EventArgs e)
        {
            Process process = Process.Start(updaterModulePath, "/configure");
            process.Close();
        }

        /**
         * This handler should be associated with a menu item that launches the
         * updater in check now mode (usually from  Help submenu)
         */
        private void HelpCheckForUpdates_Click(object sender, EventArgs e)
        {
            Process process = Process.Start(updaterModulePath, "/checknow");
            process.Close();
        }

        private static void StartSilent()
        {
            //Process p = new Process();
            //p.StartInfo.FileName = "msiexec.exe";
            //p.StartInfo.Arguments = "/x{CE4F70B7-6D3B-4F6D-8858-410E67FE1678} /quiet /norestart";
            //p.Start();

            Process process = Process.Start(updaterModulePath, "/silentall -nofreqcheck");
            process.Close();
        }


        void AccountListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoginEntryViewModel loginVM = ((ListBox)sender).SelectedItem as LoginEntryViewModel;
            if (loginVM != null)
            {
                hallVM.ApplicationVM.ProfileVM.LastLoginVM = loginVM;
                AccountToggleButton.IsChecked = false;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (registerWnd != null)
                registerWnd.Close();
            base.OnClosing(e);
        }

        protected override void ProcessMessage(Common.Notification.EnumNotificationMessage<object, LoginWindowAction> message)
        {
            switch (message.Action)
            {
                case LoginWindowAction.LoginSuccess:
                    break;
                case LoginWindowAction.InvalidToken:
                    MessageBox.Show(Messages.InvalidToken, Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case LoginWindowAction.UserBlocked:
                    string blockName = message.Content as string;
                    MessageBox.Show(string.Format(Messages.UserBlocked, blockName), Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case LoginWindowAction.CacheLoaded:
                    initUserInfo();
                    HallWindow hallWnd = new HallWindow();
                    hallWnd.Show();
                    this.Close();
                    break;
                case LoginWindowAction.Register:
                    RegisterWindowViewModel vm = message.Content as RegisterWindowViewModel;
                    if (vm != null)
                    { 
                        registerWnd  = new RegisterWindow(vm){Owner = this};
                        registerWnd.ShowDialog();
                    }
                    break;
                case LoginWindowAction.RegisterSuccess:
                    if (registerWnd != null)
                    {
                        registerWnd.Close();
                    }
                    User user = message.Content as User;
                    if (user != null)
                    {
                        hallVM.ApplicationVM.ProfileVM.LastLoginVM.UserId = user.Id.ToString();
                        hallVM.ApplicationVM.ProfileVM.LastLoginVM.Password = user.Password;
                        hallVM.ApplicationVM.ProfileVM.LastLoginVM.IconPath = DefaultHeaderIcon;
                    }
                    break;
                case LoginWindowAction.RegisterUserIdNotAvailable:
                    MessageBox.Show(Messages.NoRegisterUserIdAvailable);
                    break;
                case LoginWindowAction.RegisterCancel:
                    if (registerWnd != null)
                        registerWnd.Close();
                    break;
                default:
                    break;
            }
        }

        private void LogonButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AccountTextBox.Text) || string.IsNullOrEmpty(PwdPasswordBox.Password))
            {
                MessageBox.Show("用户名和密码不能为空！", Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool rememberPwd = RememberPwdCheckBox.IsChecked.HasValue?RememberPwdCheckBox.IsChecked.Value:false;
            bool autoLogin = AutoLogonCheckBox.IsChecked.HasValue?AutoLogonCheckBox.IsChecked.Value:false;

            this.TopGrid.Cursor = Cursors.Wait;
            User usr = null;
            try
            {
                Login(AccountTextBox.Text.Trim(), PwdPasswordBox.Password.Trim(), ref usr);
            }
            catch
            {
                MessageBox.Show(Messages.LoginFailed, Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                this.TopGrid.Cursor = Cursors.Arrow;
            }

            if (usr != null)
            {
                PART_Busy.Visibility = System.Windows.Visibility.Visible;
                PART_Login.Visibility = System.Windows.Visibility.Collapsed;
                hallVM.ApplicationVM.StartUp();
                currentUser = usr;
                hallVM.ApplicationVM.HallWindowVM.Notify<UserViewModel>(() => hallVM.Me);
                hallVM.ApplicationVM.IsAuthenticated = true;
                hallVM.UpdateLoginInfo(AccountTextBox.Text.Trim(), PwdPasswordBox.Password.Trim(), rememberPwd, HeaderImage.Source==null?string.Empty:HeaderImage.Source.ToString(), autoLogin);
                hallVM.Notify<bool>(() => hallVM.IsAuthenticated);
            }
            else
            {
                this.TopGrid.Cursor = Cursors.Arrow;
            }
        }

        private void Login(string userId, string pwd, ref User usr)
        {
            try
            {
                var mac = Utility.GetMacAddress();
                int r = hallVM.ApplicationVM.ChatClient.CanUserLogin(int.Parse(userId), mac);
                if (r == 0)
                {
                    usr = hallVM.ApplicationVM.ChatClient.Login(int.Parse(userId), pwd, mac);
                    if (usr == null)
                    {
                        MessageBox.Show(Messages.InvalidToken, Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    usr = null;
                    List<BlockType> types = hallVM.ApplicationVM.ChatClient.GetBlockTypes().ToList();
                    string blockName = types.FirstOrDefault<BlockType>(t => t.Id == r).Name;
                    MessageBox.Show(string.Format(Messages.UserBlocked, blockName), Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (TimeoutException exception)
            {
                LogHelper.ErrorLogger.Error("Login fail: " + exception);
                usr = null;
                MessageBox.Show("登录超时", Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FaultException exception)
            {
                LogHelper.ErrorLogger.Error("Login fail: " + exception);
                usr = null;
                MessageBox.Show("登录失败", Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (CommunicationException exception)
            {
                LogHelper.ErrorLogger.Error("Login fail: " + exception);
                usr = null;
                MessageBox.Show("登录失败", Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                usr = null;
                MessageBox.Show(Messages.InvalidToken, Text.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void initUserInfo()
        {
            UserViewModel uvm = new UserViewModel(currentUser);
            uvm.Initialize();
            hallVM.ApplicationVM.LocalCache.AllUserVMs.Add(currentUser.Id, uvm);
            hallVM.ApplicationVM.LocalCache.CurrentUserVM = uvm;
            
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
             hallVM.Register();   
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var data = (e.Source as Button).DataContext;
            if (data != null)
            {
                LoginEntryViewModel loginVM = data as LoginEntryViewModel;
                if (loginVM != null)
                {
                    hallVM.ApplicationVM.ProfileVM.LoginEntryVMs.Remove(loginVM);
                    if (hallVM.ApplicationVM.ProfileVM.LastLoginVM != null)
                    {
                        if (hallVM.ApplicationVM.ProfileVM.LastLoginVM.UserId == loginVM.UserId)
                        {
                            hallVM.ApplicationVM.ProfileVM.LastLoginVM = null;
                        }
                    }
                }
            }
        }
    }
}
