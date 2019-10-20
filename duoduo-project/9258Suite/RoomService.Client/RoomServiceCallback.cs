using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoYoStudio.Model.Chat;
using YoYoStudio.Model.Core;
using YoYoStudio.Model.Media;

namespace YoYoStudio.RoomService.Client
{
    public class RoomServiceCallback :IRoomServiceCallback
    {
        private Action<int, User> userEnteredRoomEvent;
        public event Action<int, User> UserEnteredRoomEvent
        {
            add
            {
                if (userEnteredRoomEvent == null || !userEnteredRoomEvent.GetInvocationList().Contains(value))
                {
                    userEnteredRoomEvent += value;
                }
            }
            remove
            {
                userEnteredRoomEvent -= value;
            }
        }

        private Action<int, int> userLeftRoomEvent;
        public event Action<int, int> UserLeftRoomEvent
        {
            add
            {
                if (userLeftRoomEvent == null || !userLeftRoomEvent.GetInvocationList().Contains(value))
                {
                    userLeftRoomEvent += value;
                }
            }
            remove
            {
                userLeftRoomEvent -= value;
            }
        }

        private Action<int, RoomMessage> roomMessageReceivedEvent;
        public event Action<int, RoomMessage> RoomMessageReceivedEvent
        {
            add
            {
                if (roomMessageReceivedEvent == null || !roomMessageReceivedEvent.GetInvocationList().Contains(value))
                {
                    roomMessageReceivedEvent += value;
                }
            }
            remove
            {
                roomMessageReceivedEvent -= value;
            }
        }

        private Action<int, MicStatusMessage> micStatusMessageReceivedEvent;
        public event Action<int, MicStatusMessage> MicStatusMessageReceivedEvent
        {
            add
            {
                if (micStatusMessageReceivedEvent == null || !micStatusMessageReceivedEvent.GetInvocationList().Contains(value))
                {
                    micStatusMessageReceivedEvent += value;
                }
            }
            remove
            {
                micStatusMessageReceivedEvent -= value;
            }
        }

        private Action<int, int, int, int> commandReceivedEvent;
        public event Action<int, int, int, int> CommandReceivedEvent
        {
            add
            {
                if (commandReceivedEvent == null || !commandReceivedEvent.GetInvocationList().Contains(value))
                {
                    commandReceivedEvent += value;
                }
            }
            remove
            {
                commandReceivedEvent -= value;
            }
        }

        private Action<int, int, int> videoStateChangedEvent;
        public event Action<int, int, int> VideoStateChangedEvent
        {
            add
            {
                if (videoStateChangedEvent == null || !videoStateChangedEvent.GetInvocationList().Contains(value))
                {
                    videoStateChangedEvent += value;
                }
            }
            remove
            {
                videoStateChangedEvent -= value;
            }
        }

        private Action<int, int, int> audioStateChangedEvent;
        public event Action<int, int, int> AudioStateChangedEvent
        {
            add
            {
                if (audioStateChangedEvent == null || !audioStateChangedEvent.GetInvocationList().Contains(value))
                {
                    audioStateChangedEvent += value;
                }
            }
            remove
            {
                audioStateChangedEvent -= value;
            }
        }

        public event Action<int, int, string> StartMusicEvent;
        public event Action<int, int> StopMusicEvent;
        public event Action<int, int,bool > TogglePauseMusicEvent;
        public event Action<int, int, int> SetPlayPositionEvent;
        public event Action<int, int, int> SetMusicVolumeEvent;
        public event Action<int,int> ReportMusicStatusEvent;
        public event Action<int, MusicStatus> UpdateMusicStatusEvent;

        public void UserEnteredRoom(int roomId, Model.Core.User user)
        {
            userEnteredRoomEvent(roomId, user);
        }

        public void UserLeftRoom(int roomId, int userId)
        {
            userLeftRoomEvent(roomId, userId);
        }

        public void RoomMessageReceived(int roomId, Model.Chat.RoomMessage message)
        {
            roomMessageReceivedEvent(roomId, message);
        }

        public void MicStatusMessageReceived(int roomId, Model.Chat.MicStatusMessage message)
        {
            micStatusMessageReceivedEvent(roomId, message);
        }


        public void CommandMessageReceived(int roomId, int cmdId, int sourceUserId, int targetUserId)
        {
            commandReceivedEvent(roomId, cmdId, sourceUserId, targetUserId);
        }

        public void VideoStateChanged(int roomId, int senderId, int state)
        {
            videoStateChangedEvent(roomId, senderId, state);
        }

        public void AudioStateChanged(int roomId, int senderId, int state)
        {
            audioStateChangedEvent(roomId, senderId, state);
        }

        public void StartMusic(int roomId, int userId, string fileName)
        {
            if (StartMusicEvent != null)
            {
                StartMusicEvent(roomId, userId, fileName);
            }
        }
        public void StopMusic(int roomId, int userId)
        {
            if (StopMusicEvent != null)
            {
                StopMusicEvent(roomId, userId);
            }
        }
        public void TogglePauseMusic(int roomId, int userId,bool paused)
        {
            if (TogglePauseMusicEvent != null)
            {
                TogglePauseMusicEvent(roomId, userId, paused);
            }
        }

        public void SetPlayPosition(int roomId, int userId, int pos)
        {
            if (SetPlayPositionEvent != null)
            {
                SetPlayPositionEvent(roomId, userId, pos);
            }
        }

        public void SetMusicVolume(int roomId, int userId, int volume)
        {
            if (SetMusicVolumeEvent != null)
            {
                SetMusicVolumeEvent(roomId, userId, volume);
            }
        }

        public void ReportMusicStatus(int roomId, int requestUserId)
        {
            if (ReportMusicStatusEvent != null)
            {
                ReportMusicStatusEvent(roomId, requestUserId);
            }
        }

        public void UpdateMusicStatus(int roomId, MusicStatus status)
        {
            if (UpdateMusicStatusEvent != null)
            {
                UpdateMusicStatusEvent(roomId, status);
            }
        }
    }
}
