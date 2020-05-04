using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace YoYoStudio.Common.Rtmp.Audio
{
    public class StreamProcessPublishModel: StreamProcessModel
    {
    }

    public class StreamProcessPublish:StreamProcess
    {
        private StreamProcessPublishModel processPublishModel;
        private int frameDroppedCount;
        private int warningTimes;
        private Action<int,string> publishExitAction;
        private Action<string,int> publishErrorAction;
        
        private int publishProcessId;
        private int publisherId;
       

        public StreamProcessPublish(StreamProcessPublishModel publishModel)
            :base(new StreamProcessModel()
            {
                RoomId = publishModel.RoomId,
                AudioDeviceName = publishModel.AudioDeviceName,
                AudioSample = publishModel.AudioSample,
            })
        {
            processPublishModel = publishModel;
            frameDroppedCount = 0;
            warningTimes = 0;
        }

        public bool Publish(int publisherId,Action<int,string> exitAction, Action<string,int> errorAction)
        {
            this.publisherId = publisherId;
            publishErrorAction = errorAction;
            publishExitAction = exitAction;
            KillProcess(publishProcessId);

            using (Process pro = new Process())
            {
                string publishMD5Code = Utility.GetMD5String(publisherId.ToString());
                string publishRtmpPath = audioRtmpBase + "/" + processPublishModel.RoomId + "/" + publisherId + "/" + publishMD5Code;
                string arg1 = "\"" + processPublishModel.AudioDeviceName + "\"";
                string arg2 = "\"" + publishRtmpPath + "\"";
                pro.StartInfo.FileName = GetPublishAudioBat();
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.Verb = "runas";
                pro.EnableRaisingEvents = true;
                pro.Exited += Pro_Publish_Exited;
                pro.StartInfo.RedirectStandardError = true;
                pro.ErrorDataReceived += Pro_Publish_ErrorDataReceived;
                //pro.StartInfo.Arguments = "-f dshow -i audio=" + arg1 + " -b:a 64k -fflags nobuffer -y -f flv " + arg2;
                pro.StartInfo.Arguments = " -re -f dshow -i audio=" + arg1 + " -ar " + processPublishModel.AudioSample + " -f flv " + arg2;
                try
                {
                    pro.Start();
                    pro.BeginErrorReadLine();
                    publishProcessId = pro.Id;
                }
                catch (Exception ex)
                {
                    LogHelperRtmp.ErrorLogger.Error(nameof(Publish), ex);
                    return false;
                }
            }

            return true;
        }

        public bool StopPublish()
        {
            var killRes = KillProcess(publishProcessId);
            return killRes == 0;
        }

        private void Pro_Publish_Exited(object sender, EventArgs e)
        {
            string msg = nameof(Pro_Publish_Exited) + $"Room: {processPublishModel.RoomId} publish exits: " + e.ToString();
            LogHelperRtmp.ErrorLogger.Error(msg);
            publishExitAction(publisherId, msg);
        }

        private void Pro_Publish_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && e.Data != null)
            {
                string msg = nameof(Pro_Publish_ErrorDataReceived) + $"Room: {processPublishModel.RoomId} publish: " + e.Data.ToString();
                if (msg.Contains("size=") && msg.Contains("time=") && msg.Contains("bitrate=") && msg.Contains("speed="))
                {
                    return;
                }
                else if (msg.Contains("frame dropped!"))
                {
                    ++frameDroppedCount;
                    LogHelperRtmp.ErrorLogger.Error(msg);
                }
                else if (msg.Contains("Last message repeated"))
                {
                    frameDroppedCount += 5;
                    LogHelperRtmp.ErrorLogger.Error(msg);
                }
                else if(msg.Contains("Conversion failed!"))
                {
                    LogHelperRtmp.ErrorLogger.Error(msg);
                    publishExitAction(publisherId, msg);
                    frameDroppedCount = 0;
                    warningTimes = 0;
                }
                else
                {
                    LogHelperRtmp.InfoLogger.Info(msg);
                }
                if(frameDroppedCount >= 120)
                {
                    warningTimes++;
                    if (warningTimes <= 3)
                    {
                        publishErrorAction("msg", warningTimes);
                        frameDroppedCount = 0;
                    }
                    else if(warningTimes >= 5)
                    {
                        StopPublish();
                        publishExitAction(publisherId, msg);
                        frameDroppedCount = 0;
                        warningTimes = 0;
                    }
                }
            }
        }
    }
}
