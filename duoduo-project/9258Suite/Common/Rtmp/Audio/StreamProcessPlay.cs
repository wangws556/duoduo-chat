using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace YoYoStudio.Common.Rtmp.Audio
{
    public class StreamProcessPlayModel: StreamProcessModel
    {
        public bool AudioSync { get; set; }
    }
    public class StreamProcessPlay:StreamProcess
    {
        private StreamProcessPlayModel streamProcessPlayModel;
        private Action<string> playExitAction;
        private Action<string> playErrorAction;
        private int playProcessId;

        public StreamProcessPlay(StreamProcessPlayModel streamProcessPlayModel)
            : base(new StreamProcessModel() {
                RoomId = streamProcessPlayModel.RoomId,
                AudioDeviceName = streamProcessPlayModel.AudioDeviceName,
                AudioSample = streamProcessPlayModel.AudioSample
            })
        {
            streamProcessPlayModel = streamProcessPlayModel;
        }

        public bool Play(int publisherId, Action<string> exitAction, Action<string> errorAction)
        {
            playExitAction = exitAction;
            playErrorAction = errorAction;

            // 杀死已有的ffmpeg进程，不要加.exe后缀
            KillProcess(playProcessId);

            using (Process pro = new Process())
            {
                string publishMD5Code = Utility.GetMD5String(publisherId.ToString());
                string publishRtmpPath = audioRtmpBase + "/" + streamProcessPlayModel.RoomId + "/" + publisherId + "/" + publishMD5Code;
                string arg1 = "\"" + publishRtmpPath + "\"";
                pro.StartInfo.FileName = GetPlayAudioBat();
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.Verb = "runas";
                pro.EnableRaisingEvents = true;
                pro.Exited += Pro_Play_Exited;
                pro.StartInfo.RedirectStandardError = true;
                pro.ErrorDataReceived += Pro_Play_ErrorDataReceived;
                if (streamProcessPlayModel.AudioSync)
                {
                    pro.StartInfo.Arguments = " -sync ext " + arg1 + " -nodisp -autoexit";
                }
                else
                {
                    pro.StartInfo.Arguments = " " + arg1 + " -nodisp -autoexit";
                }

                try
                {
                    pro.Start();
                    pro.BeginErrorReadLine();
                    playProcessId = pro.Id;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLogger.Error(nameof(Play), ex);
                    return false;
                }

            }

            return true;
        }

        public void StopPlay()
        {
            KillProcess(playProcessId);
        }

        private void Pro_Play_Exited(object sender, EventArgs e)
        {
            string msg = nameof(Pro_Play_Exited) + $"Room: {streamProcessPlayModel.RoomId} play exits: " + e.ToString();
            LogHelper.ErrorLogger.Error(msg);
            playExitAction(msg);
        }

        private void Pro_Play_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string msg = nameof(Pro_Play_Exited) + $"Room: {streamProcessPlayModel.RoomId} play error: " + e.ToString();
            LogHelper.ErrorLogger.Error(msg);
            playErrorAction(msg);
        }
    }
}
