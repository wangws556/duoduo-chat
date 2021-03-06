﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using YoYoStudio.ChatService.Client;
using System.Timers;
using YoYoStudio.Model.Core;
using YoYoStudio.Common.Wcf;
using System.Threading.Tasks;
using YoYoStudio.Model.Chat;
using YoYoStudio.Model;
using System.Globalization;
using YoYoStudio.Model.Media;
using System.Configuration;
using System.IO;

namespace YoYoStudio.RoomService.Library
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, ConfigurationName = Const.RoomServiceName, InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    public partial class RoomService : WcfService, IRoomService, IDisposable
    {
        //public static void Configure(ServiceConfiguration config)
        //{
        //    try
        //    {
        //        log4net.Config.XmlConfigurator.Configure();
        //        logger.Debug($"Room Service start loading configuration {DateTime.Now}");
        //        Initialize();
        //        ExeConfigurationFileMap cfMap;
        //        cfMap = new ExeConfigurationFileMap() { ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config") };
        //        var cf = ConfigurationManager.OpenMappedExeConfiguration(cfMap, ConfigurationUserLevel.None);
        //        config.LoadFromConfiguration(cf);
        //        logger.Debug($"Room Service load configuration successfully {DateTime.Now}");
        //    }
        //    catch(Exception ex)
        //    {
        //        logger.Error($"Room Service load configuration failed: {ex.Message}");
        //        throw;
        //    }
        //}
        public RoomService()
        {
            client.InnerChannel.Closed += InnerChannel_Closed;
            client.InnerDuplexChannel.Closed += InnerDuplexChannel_Closed;
            client.InnerChannel.Closing += InnerChannel_Closing;
            client.InnerDuplexChannel.Closing += InnerDuplexChannel_Closing;
        }

        private void InnerDuplexChannel_Closing(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InnerChannel_Closing(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InnerChannel_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #region Private Members

        private UserNCallback unc;

        void Channel_Faulted(object sender, EventArgs e)
        {
            try
            {
                LeaveRoom();
                Dispose();
            }
            catch(Exception ex)
            {
                logger.Error(nameof(Channel_Faulted), ex);
            }
        }

        private void LeaveRoom(int roomId, int userId)
        {
            try
            {
                if (userCache[roomId].ContainsKey(userId))
                {
                    userCache[roomId].Remove(userId);
                }
                if (musicCache.ContainsKey(roomId))
                {
                    if (musicCache[roomId].PlayerId == userId)
                        DownPlayMusic(roomId);
                }
                for (int i = (int)MicType.None + 1; i < (int)MicType.Max; i++)
                {
                    var micType = (MicType)i;
                    var mics = micCache[roomId][micType].Values;
                    var mic = mics.FirstOrDefault(m => m.UserId == userId);
                    if (mic != null)
                    {
                        mic.Reset();
                    }
                }
                BroadCast(roomId, (u) => u.Callback.UserLeftRoom(roomId, userId), userId);
            }
            catch(Exception ex)
            {
                logger.Error(nameof(LeaveRoom), ex);
            }
        }

        private void LeaveRoom()
        {
            if (unc.RoomId > 0)
            {
                if (userCache.ContainsKey(unc.RoomId))
                {
                    LeaveRoom(unc.RoomId,unc.User.Id);
                }
            }
        }

        private void BroadCastHornMsg(int roomGroupId, Action<UserNCallback> act, int senderId)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (roomGroupId == -1)
                        {
                            foreach (var room in userCache)
                            {
                                BroadCast(-1, act, -1);
                            }
                        }
                        else
                        {
                            var roomGroup = cache.RoomGroups.FirstOrDefault(r => r.Id == roomGroupId);
                            if (roomGroup != null)
                            {
                                roomGroup.SubRoomGroups.ForEach((subGroup) =>
                                    {
                                        BroadCastHornMsg(subGroup.Id, act, senderId);
                                    });
                                roomGroup.Rooms.ForEach((room) =>
                                    {
                                        BroadCast(room.Id, act, -1);
                                    });
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.Error(nameof(BroadCastHornMsg), ex);
                    }
                });
        }

        private void BroadCast(int roomId, Action<UserNCallback> act, int senderId)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (roomId == -1) //broadcase the message to all the rooms
                        {
                            foreach (var room in userCache)
                            {
                                BroadCast(room.Key, act, senderId);
                            }
                        }
                        else
                        {
                            var users = userCache[roomId].Values;
                            foreach (var u in users)
                            {
                                if (u.User.Id != senderId)
                                {
                                    try
                                    {
                                        act(u);
                                    }
                                    catch(Exception ex)
                                    {
                                        logger.Error(nameof(act), ex);
                                        LeaveRoom(roomId, u.User.Id);
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.Error(nameof(BroadCast), ex);
                    }
                });
        }

        #endregion

        #region Service Implementation

        public void KeepAlive()
        {
            try
            {
                client.KeepAlive();
            }
            catch(Exception ex)
            {
                logger.Error(nameof(KeepAlive), ex);
            }
        }

        public bool EnterRoom(int roomId, User user)
        {
            if (userCache.ContainsKey(roomId))
            {
                try
                {
                    var info = client.GetUserInfo(user.Id);
                    OperationContext.Current.Channel.Faulted += Channel_Faulted;
                    this.unc = new UserNCallback { User = user, RoomId = roomId, Callback = OperationContext.Current.GetCallbackChannel<IRoomServiceCallback>(), UserInfo = info };
                    userCache[roomId][user.Id] = unc;
                    BroadCast(roomId, (u) => u.Callback.UserEnteredRoom(roomId, unc.User), user.Id);
                }
                catch(Exception ex)
                {
                    logger.Error(nameof(EnterRoom),ex);
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool ExecuteCommand(int roomId, int cmdId, int targetUserId)
        {
            bool result = false;
            if (userCache[roomId].ContainsKey(targetUserId))
            {
                var other = userCache[roomId][targetUserId];
                if (other != null)
                {
                    if (cache.HasCommand(roomId, cmdId, unc.User.Id, unc.UserInfo.Role_Id, other.UserInfo.Role_Id))
                    {
                        ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                        try
                        {
                            if (client.ExecuteCommand(roomId, cmdId, unc.User.Id, targetUserId))
                            {
                                if (Applications._9258App.CommandNeedsBroadcast(cmdId))
                                {
                                    BroadCast(roomId, u => u.Callback.CommandMessageReceived(roomId, cmdId, unc.User.Id, targetUserId), unc.User.Id);
                                }
                                else
                                {
                                    other.Callback.CommandMessageReceived(roomId, cmdId, unc.User.Id, targetUserId);
                                }
                                result = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            logger.Error($"RoomServer room {roomId} execute command {roomId} targetUserId {targetUserId} failed: ",ex);
                        }
                        finally
                        {
                            client.Close();
                        }
                    }
                }
            }
            else
            {
                LeaveRoom(roomId, targetUserId);
            }
            return result;
        }

        public void LeaveRoom(int roomId)
        {
            LeaveRoom(roomId, unc.User.Id);
        }

        public int GetOnlineUserCount(int roomId)
        {
            if (userCache.ContainsKey(roomId))
            {
                return userCache[roomId].Count;
            }
            return 0;
        }

        public User GetUser(int userId)
        {
            foreach (var roomUsers in userCache)
            {
                var userNC = roomUsers.Value[userId];
                if (userNC != null)
                    return userNC.User;
            }
            return null;
        }

        public List<User> GetRoomUsers(int roomId)
        {
            List<User> resultList = new List<User>();
            try
            {
                resultList = userCache[roomId].Values.Select(u => u.User).ToList();
            }
            catch(Exception ex)
            {
                logger.Error(nameof(GetRoomUsers), ex);
            }
            return resultList;
        }

        public void SendRoomMessage(int roomId, RoomMessage message)
        {
            MessageResult result = MessageResult.Succeed;
            //TO Adjust the message, for example, whether it should be scroll, horn, etc.
            switch (message.MessageType)
            {
                case RoomMessageType.AnnoucementMessage:
                    break;
                case RoomMessageType.StampMessage:
                    break;
                case RoomMessageType.PublicChatMessage:
                     BroadCast(roomId, (u) => u.Callback.RoomMessageReceived(roomId, message),unc.User.Id);
                     break;
                case RoomMessageType.PrivateChatMessage:
                    var uncLocalPri = userCache[roomId][message.ReceiverId];
                    if (uncLocalPri != null)
                    {
                        try
                        {
                            uncLocalPri.Callback.RoomMessageReceived(roomId, message);
                        }
                        catch
                        {
                            LeaveRoom(roomId, uncLocalPri.User.Id);
                        }
                    }
                        break;
                case RoomMessageType.HornMessage:
                    ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                    try
                    {
                        result = client.SendHornMessage(roomId, unc.User.Id, Applications._9258App.FrontendCommands.HornCommandId);
                    }
                    catch(Exception ex)
                    {
                        logger.Error($"HornMessage failed: {ex.Message}");
                        throw;
                    }
                    finally
                    {
                        client.Close();
                    }
                    message.MsgResult = result;
                      if (result == MessageResult.Succeed)
                    {
                        var room = cache.Rooms.FirstOrDefault(r => r.Id == roomId);
                        if (room != null)
                        {
                            var roomGroupId = room.RoomGroup_Id;
                            if (roomGroupId != null)
                            {
                                var roomGroup = cache.RoomGroups.FirstOrDefault(r=>r.Id == roomGroupId);
                                if (roomGroup != null)
                                {
                                    var hallGroupId = roomGroup.ParentGroup_Id;
                                    if (hallGroupId != null)
                                    {
                                        BroadCastHornMsg((int)hallGroupId, (u) => u.Callback.RoomMessageReceived((int)hallGroupId, message), unc.User.Id);
                                    }
                                }
                            }
                        }
                    }
                    else
                        unc.Callback.RoomMessageReceived(roomId, message);
                    break;
                case RoomMessageType.HallHornMessage:
                    ChatServiceClient client2 = new ChatServiceClient(new ChatServiceCallback());
                    try
                    {
                        result = client2.SendHornMessage(roomId, unc.User.Id, Applications._9258App.FrontendCommands.HallHornCommandId);
                    }
                    catch(Exception ex)
                    {
                        logger.Error($"HallHornMessage failed: {ex.Message}");
                        throw;
                    }
                    finally
                    {
                        client2.Close();
                    }
                    message.MsgResult = result;
                    if (result == MessageResult.Succeed)
                    {
                        var room2 = cache.Rooms.FirstOrDefault(r => r.Id == roomId);
                        if (room2 != null)
                        {
                            var roomGroupId = room2.RoomGroup_Id;
                            if (roomGroupId != null)
                            {
                                var roomGroup = cache.RoomGroups.FirstOrDefault(r=>r.Id == roomGroupId);
                                if (roomGroup != null)
                                {
                                    var hallGroupId = roomGroup.ParentGroup_Id;
                                    if (hallGroupId != null)
                                    {
                                        var hallGroup = cache.RoomGroups.FirstOrDefault(r => r.Id == hallGroupId);
                                        if (hallGroup != null)
                                        {
                                            var tempId = hallGroup.ParentGroup_Id;
                                            if (tempId != null)
                                            {
                                                BroadCastHornMsg((int)tempId, (u) => u.Callback.RoomMessageReceived((int)tempId, message), unc.User.Id);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                        unc.Callback.RoomMessageReceived(roomId, message);
                    break;
                case RoomMessageType.GlobalHornMessage:
                    ChatServiceClient client3 = new ChatServiceClient(new ChatServiceCallback());
                    try
                    {
                        result = client3.SendHornMessage(roomId, unc.User.Id, Applications._9258App.FrontendCommands.GlobalHornCommandId);
                    }
                    catch(Exception ex)
                    {
                        logger.Error($"GlobalHornMessage failed: {ex.Message}");
                        throw;
                    }
                    finally
                    {
                        client3.Close();
                    }
                    message.MsgResult = result;
                    if (result == MessageResult.Succeed)
                    {
                        BroadCastHornMsg(-1, (u) => u.Callback.RoomMessageReceived(roomId, message), unc.User.Id);
                    }
                    else
                        unc.Callback.RoomMessageReceived(roomId, message);
                    break;
            }
        }

       

        public void OnMic(int roomId, MicType micType, int suggestedIndex = -1)
        {
            try
            {
                MicStatusMessage msg = null;
                int index = GetAvailableMicStatus(roomId, micType, unc.User.Id, suggestedIndex, out msg);
                if (msg != null)
                {
                    msg.UserId = unc.User.Id;
                    msg.MicAction = MicAction.OnMic;
                    msg.MicType = micType;
                    if (msg.MicStatus != MicStatusMessage.MicStatus_Queue)
                    {
                        if (index >= 0)
                        {
                            msg.MicIndex = index;
                            msg.StreamGuid = Guid.NewGuid().ToString();
                            msg.MicStatus = MicStatusMessage.MicStatus_Video;
                        }
                    }
                    unc.Callback.MicStatusMessageReceived(roomId, msg);
                    BroadCast(roomId, (u) => u.Callback.MicStatusMessageReceived(roomId, msg), unc.User.Id);
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(OnMic), ex);
            }
        }

        public void DownMic(int roomId, MicType micType, int index)
        {
            try
            {
                var mics = micCache[roomId][micType];
                if (index < 0)
                {
                    var usr = mics.Values.FirstOrDefault(u => u.UserId == unc.User.Id);
                    if (usr != null)
                    {
                        index = usr.MicIndex;
                    }
                }
                if (index >= 0)
                {
                    if (mics[index].UserId == unc.User.Id)
                    {
                        mics[index].Reset();
                        var msg = new MicStatusMessage
                        {
                            MicAction = Model.Chat.MicAction.DownMic,
                            MicIndex = index,
                            MicStatus = MicStatusMessage.MicStatus_Off,
                            MicType = micType,
                            UserId = unc.User.Id
                        };
                        BroadCast(roomId, (u) => u.Callback.MicStatusMessageReceived(roomId, msg), unc.User.Id);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(DownMic), ex);
            }
        }

        public void PublishAudio(int roomId, int userId, MicType micType, AudioStatusType status)
        {
            try
            {
                MicStatusMessage msg = GetUserMicStatus(roomId, userId, (MicType)(micType));
                if(msg != null)
                {
                    msg.AudioStatus = status;
                    unc.Callback.AudioPublishStatusMessageReceived(roomId, msg);
                    BroadCast(roomId, (u) => u.Callback.AudioPublishStatusMessageReceived(roomId, msg), unc.User.Id);
                }

            }
            catch (Exception ex)
            {
                logger.Error(nameof(PublishAudio), ex);
            }
        }

        public void ToggleVideo(int roomId, MicType micType)
        {
            try
            {
                var micS = GetUserMicStatus(roomId, unc.User.Id, micType);
                if (micS != null && micS.MicStatus != MicStatusMessage.MicStatus_Off)
                {
                    if ((micS.MicStatus & MicStatusMessage.MicStatus_Video) > 0)
                    {
                        micS.MicStatus = micS.MicStatus & (~MicStatusMessage.MicStatus_Video);
                    }
                    else
                    {
                        micS.MicStatus = micS.MicStatus | MicStatusMessage.MicStatus_Video;
                    }
                    var msg = new MicStatusMessage
                    {
                        MicAction = Model.Chat.MicAction.Toggle,
                        MicStatus = micS.MicStatus,
                        UserId = unc.User.Id
                    };
                    BroadCast(roomId, (u) => u.Callback.MicStatusMessageReceived(roomId, msg), unc.User.Id);
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(ToggleVideo), ex);
            }
        }

        public void ToggleAudio(int roomId, MicType micType)
        {
            try
            {
                var micS = GetUserMicStatus(roomId, unc.User.Id, micType);
                if (micS != null && micS.MicStatus != MicStatusMessage.MicStatus_Off)
                {
                    if ((micS.MicStatus & MicStatusMessage.MicStatus_Audio) > 0)
                    {
                        micS.MicStatus = micS.MicStatus & (~MicStatusMessage.MicStatus_Audio);
                    }
                    else
                    {
                        micS.MicStatus = micS.MicStatus | MicStatusMessage.MicStatus_Audio;
                    }

                    var msg = new MicStatusMessage
                    {
                        MicAction = Model.Chat.MicAction.Toggle,
                        MicStatus = micS.MicStatus,
                        UserId = unc.User.Id
                    };

                    BroadCast(roomId, (u) => u.Callback.MicStatusMessageReceived(roomId, msg), unc.User.Id);
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(ToggleAudio), ex);
            }
        }

        public RoomMessage GetRoomMessage(RoomMessageType msgType)
        {
            return RoomsPermanentMsg;
        }

        public Dictionary<int, MicStatusMessage> GetMicUsers(int roomId, MicType micType)
        {
            Dictionary<int, MicStatusMessage> result = new Dictionary<int, MicStatusMessage>();
            try
            {
                //lock (micCacheLocker[roomId])
                {
                    foreach (var pair in micCache[roomId][micType])
                    {
                        if (pair.Value.MicStatus != MicStatusMessage.MicStatus_Off && pair.Value.UserId > 0)
                        {
                            result.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(GetMicUsers), ex);
            }
            return result;
        }

        public List<MicStatusMessage> GetMicQueue(int roomId)
        {
            if (micQueue.ContainsKey(roomId))
            {
                return micQueue[roomId].ToList();
            }
            return new List<MicStatusMessage>();
        }

        public bool ScoreExchange(int userId, int scoreToExchange, int moneyToGet)
        {
            bool result = false;
            if (!unc.UserInfo.Score.HasValue || unc.UserInfo.Score < scoreToExchange)
                return result;
            ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
            try
            {
                if (client.ScoreExchange(userId, scoreToExchange, moneyToGet))
                {
                    unc.UserInfo.Score -= scoreToExchange;
                    if (!unc.UserInfo.Money.HasValue)
                        unc.UserInfo.Money = 0;
                    unc.UserInfo.Money += moneyToGet;
                    result = true;
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(ScoreExchange),ex);
            }
           
            return result;
        }

		public void SendGift(int roomId, int receiverId, int giftId, int count)
		{
			var receiver = userCache[roomId][receiverId];
			SendGiftResult result = SendGiftResult.Succeed;
			if (!cache.HasCommand(roomId, Applications._9258App.FrontendCommands.SendGiftCommandId, unc.User.Id,unc.UserInfo.Role_Id,-1))
			{
				result = SendGiftResult.CannotSendGift;
			}
			else if (!cache.HasCommand(roomId, Applications._9258App.FrontendCommands.ReceiveGiftCommandId, receiverId,receiver.UserInfo.Role_Id,-1))
			{
				result = SendGiftResult.CannotReceiveGift;
			}
			else
			{
				var gift = cache.Gifts.FirstOrDefault(g => g.Id == giftId);
				try
				{
                    if (!unc.UserInfo.Money.HasValue || unc.UserInfo.Money < gift.Price * count)
                    {
                        result = SendGiftResult.NotEnoughMoney;
                    }
                    else
                    {
                        ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                        result = client.SendGift(roomId, unc.User.Id, receiverId, giftId, count);
                        if (result == SendGiftResult.Succeed)
                        {
                            unc.UserInfo.Money -= gift.Price * count;
                            if (!receiver.UserInfo.Score.HasValue)
                                receiver.UserInfo.Score = 0;
                            receiver.UserInfo.Score += gift.Score * count;
                        }
                    }
				}
				catch(Exception ex)
				{
                    logger.Error(nameof(SendGift), ex);
					result = SendGiftResult.UnkownError;
				}
			}
			
			RoomMessage msg = new RoomMessage
			{
				GiftResult = result,
				MessageType = RoomMessageType.GiftMessage,
				SenderId = unc.User.Id,
				ReceiverId = receiverId,
				Count = count,
				ItemId = giftId,
                Time = DateTime.Now.ToString("MM月dd日 HH:mm", CultureInfo.CreateSpecificCulture("zh-CN"))
			};
			switch (result)
			{
				case SendGiftResult.CannotReceiveGift:
				case SendGiftResult.CannotSendGift:
				case SendGiftResult.NotEnoughMoney:
				case SendGiftResult.UnkownError:
					unc.Callback.RoomMessageReceived(roomId, msg);
					break;
				case SendGiftResult.Succeed:
                    var gift = cache.Gifts.FirstOrDefault(g => g.Id == giftId);
                    if (count >= gift.RunWay)
                    {
                        //SenderId as -1 so the sender will receive thsi message
                        //first parameter as -1 means all the rooms will receive the message
                        BroadCast(-1, (u) => u.Callback.RoomMessageReceived(roomId, msg), -1);
                        RoomsPermanentMsg = msg;
                    }
                    else
                    {
                        //SenderId as -1 so the sender will receive thsi message
                        BroadCast(roomId, (u) => u.Callback.RoomMessageReceived(roomId, msg), -1);
                    }
					break;
			}
		}

        #endregion

        #region IDisposable Implementation

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
                disposed = true;
            }
        }

        ~RoomService()
        {
            Dispose(false);
        }

        #endregion

        public void VideoStateChanged(int roomId, int state)
        {
            BroadCast(roomId, u => u.Callback.VideoStateChanged(roomId,unc.User.Id, state), unc.User.Id);
        }

        public void AudioStateChanged(int roomId, int state)
        {
            try
            {
                var micDic = micCache[roomId][MicType.Public];
                foreach (var item in micDic)
                {
                    if (item.Value.UserId == unc.User.Id)
                    {
                        item.Value.MicStatus = state;
                    }
                }
                BroadCast(roomId, u => u.Callback.AudioStateChanged(roomId, unc.User.Id, state), unc.User.Id);
            }
            catch(Exception ex)
            {
                logger.Error(nameof(AudioStateChanged), ex);
            }
        }

        public void AudioServiceLogin(string ip, int port)
        {
            audioServiceIp = ip;
            audioServicePort = port;
            audioServiceCallback = OperationContext.Current.GetCallbackChannel<IRoomServiceCallback>();
        }

        public void AudioServiceLogOff()
        {
            audioServiceCallback = null;
        }


        public string GetAudioServiceIp()
        {
            return audioServiceIp;
        }

        public int GetAudioServicePort()
        {
            return audioServicePort;
        }

        #region for Music play

        public bool OnPlayMusic(int roomId, int userId)
        {
            if (IsMusicAvailabe(roomId, userId))
            {
                if (!musicCache.ContainsKey(roomId))
                {
                    MusicStatus musicStatus = new MusicStatus();
                    musicStatus.PlayerId = userId;
                    musicCache.Add(new KeyValuePair<int,MusicStatus>(roomId,musicStatus));
                }
                return true;

            }
            return false;
        }

        public int GetMusicPlayer(int roomId)
        {
            if (musicCache.ContainsKey(roomId))
                return musicCache[roomId].PlayerId;
            return -1;
        }

        public void DownPlayMusic(int roomId)
        {
            musicCache.Remove(roomId);
        }

        public void StartMusic(int roomId, int userId, string fileName)
        {
            musicCache[roomId].Name = fileName;
            BroadCast(roomId, u => u.Callback.StartMusic(roomId, userId, fileName), userId);
        }

        public void StopMusic(int roomId, int userId)
        {
            musicCache[roomId].Status = PlayStatus.Stoped;
            BroadCast(roomId, u => u.Callback.StopMusic(roomId, userId), userId);
        }

        public void TogglePauseMusic(int roomId, int userId,bool paused)
        {
            musicCache[roomId].Status = paused ? PlayStatus.Paused : PlayStatus.Playing;
            BroadCast(roomId, u => u.Callback.TogglePauseMusic(roomId, userId,paused), userId);
        }

        public void SetPlayPosition(int roomId, int userId, int pos)
        {
            musicCache[roomId].Position = pos;
            BroadCast(roomId, u => u.Callback.SetPlayPosition(roomId, userId, pos), userId);
        }

        public void SetMusicVolume(int roomId, int userId, int volume)
        {
            BroadCast(roomId, u => u.Callback.SetMusicVolume(roomId, userId, volume), userId);
        }

        public MusicStatus GetMusicStatus(int roomId)
        { 
            if (GetMusicPlayer(roomId) != -1)
            {
                return musicCache[roomId];
            }
            return null;
        }

        public void RequestMusicStatus(int roomId, int userId)
        {
            try
            {
                if (musicCache.ContainsKey(roomId))
                {
                    UserNCallback callBack = userCache[roomId][musicCache[roomId].PlayerId];
                    if (callBack != null)
                    {
                        callBack.Callback.ReportMusicStatus(roomId, userId);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(RequestMusicStatus), ex);
            }
        }

        public void UpadateMusicStatus(int roomId, int userId, MusicStatus status,  int targetUserId)
        {
            try
            {
                if (musicCache.ContainsKey(roomId))
                {
                    if (musicCache[roomId].PlayerId == userId)
                    {
                        musicCache[roomId] = status;

                    }
                    UserNCallback callBack = userCache[roomId][targetUserId];
                    if (callBack != null)
                    {
                        callBack.Callback.UpdateMusicStatus(roomId, status);
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(nameof(UpadateMusicStatus), ex);
            }
        }

        #endregion
    }
}
