using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using YoYoStudio.Common;
using YoYoStudio.Common.Net;
using YoYoStudio.Common.Notification;
using YoYoStudio.Media.NAudio;
using YoYoStudio.Model.Chat;

namespace YoYoStudio.Client.ViewModel
{
    public partial class RoomWindowViewModel
    {
		private AudioCodec defaultCodec = AudioCodec.UltraWideBandSpeex;
		private PlayerType defaultPlayerType = PlayerType.DirectSound;
        private UdpClient audioClient = new UdpClient();
        private IRecorder recorder;
        private Dictionary<int, INetworkPlayer> soundPlayers = new Dictionary<int, INetworkPlayer>();
        private ReaderWriterLockSlim audioRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public void InitializeAudio()
        {
			try
			{
				string codec = System.Configuration.ConfigurationManager.AppSettings["TargetAudioSampleRate"];
				defaultCodec = (AudioCodec)Enum.Parse(typeof(AudioCodec), codec);
			}
			catch { }

            ConnectAudio();
        }
        void SampleAvailableEventHandler(List<byte[]> datas)
        {
            if (datas != null &&datas.Count > 0)
            {
                UdpPacket packet = new UdpPacket(Me.Id, RoomVM.Id, UdpPacketType.Data);
                foreach (var d in datas)
                {
                    packet.Append(d);
                }
                AsyncBeginAudioSend(packet.ToBytes());
            }
        }

        private volatile bool playAll = false;

        public void StartAudioPlaying(int userId)
        {
            if (!soundPlayers.ContainsKey(userId))
            {
				if (((Me.MicStatus & MicStatusMessage.MicStatus_Audio) == MicStatusMessage.MicStatus_Off) 
					|| (!ApplicationVM.ProfileVM.AudioConfigurationVM.LoopbackRecording))
				{
					soundPlayers.Add(userId, AudioManager.CreatePlayer(defaultCodec, defaultPlayerType));
				}
            }
            soundPlayers[userId].Play();
        }

        public void StopAllAudioPlaying()
        {
            foreach (var player in soundPlayers.Values)
            {
                player.Stop();
                player.Dispose();
            }
            soundPlayers.Clear();
        }

        public void StopAudioPlaying(int userId)
        {
            if (soundPlayers.ContainsKey(userId))
            {
                soundPlayers[userId].Stop();
                soundPlayers[userId].Dispose();
                soundPlayers.Remove(userId);
            }
        }

        public void StartAudioRecording(string file = "")
        {
            //using udp recording
            try
            {
                if (ApplicationVM.ProfileVM.AudioConfigurationVM.LoopbackRecording)
                {
                    if (recorder == null)
                    {
                        recorder = AudioManager.CreateRecorder(defaultCodec, true);
                        recorder.SampleAvailableEvent += SampleAvailableEventHandler;
                    }
                    recorder.Start(file);
                }
            }
            catch
            {
            }

        }

        #region ffmpeg audio publish/play
        public bool StartAudioPublish(int publisherId)
        {
            bool result = false;
            //using ffmpeg publish
            try
            {
                if (ApplicationVM.ProfileVM.AudioConfigurationVM.LoopbackRecording)
                {
                    result = AudioPublishVM.Publish(publisherId, PublishExitAction, PublishErrorAction);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLogger.Error(nameof(StartAudioPublish), ex);
            }
            return result;
        }

        public void StartAudioPlay(int userId)
        {
            try
            {
                AudioPlayVM.Play(userId, PlayExitAction,PlayErrorAction);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLogger.Error(nameof(StartAudioPlay), ex);
            }
        }

        public bool StopPublish()
        {
            bool stopRes = false;
            try
            {
                stopRes = AudioPublishVM.StopPublish();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLogger.Error(nameof(StopPublish), ex);
            }
            return stopRes;
        }

        public void StopPlay()
        {
            try
            {
                AudioPlayVM.StopPlay();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLogger.Error(nameof(StopPlay), ex);
            }
        }

        public void PlayErrorAction(string message)
        {

        }

        public void PlayExitAction(string message)
        {

        }

        public void PublishErrorAction(string message,int warningTimes)
        {
            LogHelper.ErrorLogger.Error(nameof(PublishExitAction) + message);
            Messenger.Default.Send(new EnumNotificationMessage<object, RoomWindowAction>(RoomWindowAction.PublishWarning, message));
        }

        public void PublishExitAction(int publisherId, string message)
        {
            LogHelper.ErrorLogger.Error(nameof(PublishExitAction) + message);
            RoomClient.PublishAudio(RoomVM.Id, publisherId, MicType.Public, AudioStatusType.Off);
            Thread.Sleep(2000);
            RoomClient.PublishAudio(RoomVM.Id, publisherId, MicType.Public, AudioStatusType.On);
        }

        #endregion

        public void PauseAudioRecording()
        {
            if (recorder != null)
            {
                recorder.SampleAvailableEvent -= SampleAvailableEventHandler;
                try
                {
                    recorder.Stop();                    
                }
                catch(Exception ex)
                {
                    LogHelper.ErrorLogger.Error(nameof(PauseAudioRecording), ex);
                }
                finally
                {
                    recorder = null;
                }
            }
        }

        public void StopAudioRecording()
        {
            if (recorder != null)
            {
                recorder.SampleAvailableEvent -= SampleAvailableEventHandler;
                try
                {
                    recorder.Stop();
                }
                catch(Exception ex)
                {
                    LogHelper.ErrorLogger.Error(nameof(StopAudioRecording), ex);
                }
                finally
                {
                    recorder = null;
                }
            }
        }

        public void SetVolume(double vol)
        {
            foreach (var player in soundPlayers.Values)
            {
                player.Volume = (float)vol;
                //item.Volume = (int)vol;
            }
        }

        private void ReleaseAudio()
        {
            StopAudioRecording();
            StopAllAudioPlaying();
            DisconnectAudio();
            AudioPublishVM.StopPublish();
            AudioPlayVM.StopPlay();
        }

        private void DisconnectAudio()
        {
            UdpPacket packet = new UdpPacket(Me.Id, RoomVM.Id, UdpPacketType.Logoff);
            var logoffBytes = packet.ToBytes();

            if (audioClient != null)
            {
                if (audioClient.Client.Connected)
                {
                    try
                    {
                        audioRWLock.EnterWriteLock();
                        while (true)
                        {
                            try
                            {
                                if (audioClient != null)
                                {
                                    audioClient.Send(logoffBytes, logoffBytes.Length);
                                    IPEndPoint serverEndPoint = null;
                                    var bytes = audioClient.Receive(ref serverEndPoint);
                                    UdpPacket p = UdpPacket.FromBytes(bytes);
                                    if (p.PacketType == UdpPacketType.LogoffSuccedd)
                                    {
                                        break;
                                    }
                                }
                                else
                                    break;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        if(audioClient != null)
                            audioClient.Close();
                    }
                    catch { }
                    finally
                    {
                        audioClient = null;
                        audioRWLock.ExitWriteLock();
                    }
                }
                
            }
        }

        private void ConnectAudio()
        {
            audioClient.Connect(RoomVM.ServiceIp, ApplicationVM.LocalCache.RoomAudioServicePort);
            UdpPacket packet = new UdpPacket(Me.Id, RoomVM.Id, UdpPacketType.Login);
            var loginBytes = packet.ToBytes();
            try
            {
                audioRWLock.EnterWriteLock();
                while (true)
                {
                    try
                    {
                        audioClient.Send(loginBytes, loginBytes.Length);
                        IPEndPoint serverEndPoint = null;
                        //IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var bytes = audioClient.Receive(ref serverEndPoint);
                        UdpPacket p = UdpPacket.FromBytes(bytes);
                        if (p.PacketType == UdpPacketType.LoginSucceed)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch { }
            finally
            {
                audioRWLock.ExitWriteLock();
            }
            AsyncBeginAudioReceive();
        }

        private void AsyncBeginAudioReceive()
        {
			if (audioClient != null)
			{
				audioRWLock.EnterReadLock();
                try
				{
                    if (audioClient != null)
                    {
                        audioClient.BeginReceive(AsyncEndAudioReceive, null);
                    }
				}
				catch
				{
				}
				finally
				{
					audioRWLock.ExitReadLock();
				}
			}
        }

        private void AsyncEndAudioReceive(IAsyncResult ar)
        {
            if (audioClient != null)
            {
                audioRWLock.EnterReadLock();
                if (audioClient != null)
                {
                    try
                    {
                        IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Any, 0);
                        byte[] receivedData = audioClient.EndReceive(ar, ref remoteEP);
                        if (((Me.MicStatus & MicStatusMessage.MicStatus_Audio) == MicStatusMessage.MicStatus_Off)
                            || !ApplicationVM.ProfileVM.AudioConfigurationVM.LoopbackRecording)
                        {
                            UdpPacket packet = UdpPacket.FromBytes(receivedData);

                            if (packet.PacketType == UdpPacketType.Data)
                            {
                                if (!soundPlayers.ContainsKey(packet.UserId) && playAll)
                                {
                                    StartAudioPlaying(packet.UserId);
                                }
                                if (soundPlayers.ContainsKey(packet.UserId))
                                {
                                    foreach (var d in packet.Data)
                                    {
                                        soundPlayers[packet.UserId].AddSample(d, 0, d.Length);
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
                audioRWLock.ExitReadLock();
                AsyncBeginAudioReceive();
            }
        }

        private void AsyncBeginAudioSend(byte[] audioData)
        {
			if (audioClient != null)
			{
				audioRWLock.EnterReadLock();
				try
				{
                    if (audioClient != null)
                    {
                        audioClient.BeginSend(audioData, audioData.Length, AsyncEndAudioSend, null);
                    }
				}
				catch
				{
				}
				finally
				{
					audioRWLock.ExitReadLock();
				}
			}
        }

        private void AsyncEndAudioSend(IAsyncResult ar)
        {
			if (audioClient != null)
			{
				audioRWLock.EnterReadLock();
				try
				{
                    if (audioClient != null)
                    {
                        audioClient.EndSend(ar);
                    }
				}
				catch
				{

				}
				finally
				{
					audioRWLock.ExitReadLock();
				}
			}
        }
    }
}
