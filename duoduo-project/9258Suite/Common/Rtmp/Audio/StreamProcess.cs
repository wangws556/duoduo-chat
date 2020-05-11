using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;

namespace YoYoStudio.Common.Rtmp.Audio
{
    public class StreamProcessModel
    {
        public int RoomId { get; set; }
        public string AudioDeviceName { get; set; }
        public string AudioSample { get; set; }
        public bool AudioSync { get; set; }
        public int PublisherId { get; set; }
    }

    public class StreamProcess
    {
        public StreamProcessModel ProcessModel { get; set; }
        public string audioRtmpBase = @"rtmp://222.189.228.201:1935/live";

        #region dllimport method

        [DllImport("kernel32.dll")]
        static extern bool GenerateConsoleCtrlEvent(int dwCtrlEvent, int dwProcessGroupId);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(IntPtr handlerRoutine, bool add);
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        #endregion

        public StreamProcess(StreamProcessModel processModel)
        {
            ProcessModel = processModel;
        }

        protected int KillProcess(int processId)
        {
            if (processId != 0)
            {
                try
                {
                    AttachConsole(processId);
                    // 将控制台事件的处理句柄设为Zero，即当前进程不响应控制台事件
                    // 避免在向控制台发送【Ctrl C】指令时连带当前进程一起结束
                    SetConsoleCtrlHandler(IntPtr.Zero, true);
                    // 向控制台发送 【Ctrl C】结束指令
                    // ffmpeg会收到该指令停止录制
                    GenerateConsoleCtrlEvent(0, 0);

                    Thread.Sleep(3000);

                    // 卸载控制台事件的处理句柄，不然之后的ffmpeg调用无法正常停止
                    SetConsoleCtrlHandler(IntPtr.Zero, false);
                    // 剥离已附加的控制台
                    FreeConsole();
                    processId = 0;
                }
                catch(Exception ex)
                {
                    LogHelper.ErrorLogger.Error(nameof(KillProcess), ex);
                }
            }
            return processId;
        }
        protected string GetPublishAudioBat()
        {
            if (Utility.Is64System())
                return AppDomain.CurrentDomain.BaseDirectory + "ffmpeg\\system64\\ffmpeg.exe";
            else
                return AppDomain.CurrentDomain.BaseDirectory + "ffmpeg\\system32\\ffmpeg.exe";
        }
        protected string GetPlayAudioBat()
        {
            if (Utility.Is64System())
                return AppDomain.CurrentDomain.BaseDirectory + "ffmpeg\\system64\\ffplay.exe";
            else
                return AppDomain.CurrentDomain.BaseDirectory + "ffmpeg\\system32\\ffplay.exe";
        }
    }
}
