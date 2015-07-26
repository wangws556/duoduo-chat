using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using YoYoStudio.Client.Chat.WebPages;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common;

namespace Client.Chat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                CheckAdministrator(e);
                if(IsUpgrading())
                {
                    YoYoStudio.Client.Chat.Helper.Logger.Info("Application is upgrading " + DateTime.Now.ToLongTimeString());
                    MessageBox.Show("升级程序正在运行，请升级成功后再运行程序。");
                    Environment.Exit(0);
                }
                YoYoStudio.Client.Chat.Helper.Logger.Info("Application Started : " + DateTime.Now.ToLongTimeString());
                AllWebPages.Initialize();
            }
            catch (Exception ex)
            {
                YoYoStudio.Client.Chat.Helper.Logger.Error("OnStartup", ex);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Singleton<ApplicationViewModel>.Instance.Dispose();
            YoYoStudio.Client.Chat.Helper.Logger.Info("Application Shutdown : " + DateTime.Now.ToLongTimeString());
            base.OnExit(e);
        }

        /// <summary>
        /// 检查是否是管理员身份
        /// </summary>
        private void CheckAdministrator(StartupEventArgs e)
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                startInfo.Arguments = String.Join(" ", e.Args);
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    YoYoStudio.Client.Chat.Helper.Logger.Info("Application satrt failed due to: " + ex.Message + DateTime.Now.ToLongTimeString());
                }

                // Shut down the current process
                Environment.Exit(0);
            }
        }

        private bool IsUpgrading()
        {
            if (System.Diagnostics.Process.GetProcessesByName("update.exe").ToList().Count > 0)
                return true;
            else 
                return false;
        }
    }
}
