using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoYoStudio.Common.Notification;
using YoYoStudio.Common.Wpf.ViewModel;
using YoYoStudio.Model;
using YoYoStudio.Model.Chat;

namespace YoYoStudio.Client.ViewModel
{
    public partial class UserViewModel
    {
        #region OpenConfigurationCommand

        public SecureCommand OpenConfigurationCommand { get; set; }

        private void OpenConfigurationCommandExecute(SecureCommandArgs args)
        {
            Messenger.Default.Send<EnumNotificationMessage<object, HallWindowAction>>(
                new EnumNotificationMessage<object, HallWindowAction>(HallWindowAction.OpenConfigurationWindow));
        }

        private bool CanOpenConfigurationCommandExecute(SecureCommandArgs args)
        {
            return ApplicationVM.LocalCache.CurrentUserVM != null && ApplicationVM.LocalCache.CurrentUserVM == this;
        }

        #endregion

        #region SelectUserCommand

        public SecureCommand SelectUserCommand { get; set; }

        private void SelectUserCommandExecute(SecureCommandArgs args)
        {
            if (CanSelectUserCommandExecute(args))
            {
                RoomWindowVM.SelectUser(this);
                ApplicationVM.RoomWindowVM.CallJavaScript("addMicAndChatUser", NickName, Id);
            }
        }

        private bool CanSelectUserCommandExecute(SecureCommandArgs args)
        {
            return RoomWindowVM != null;
        }

        #endregion

        #region PublishAudioCommand

        public SecureCommand PublishAudioCommand { get; set; }

        private void PublishAudioCommandExecute(SecureCommandArgs args)
        {
            if (CanPublishAudioCommandExecute(args))
            {
                bool publishRes = this.RoomWindowVM.StartAudioPublish(this.Id);
                if (publishRes)
                {
                    AudioStatus = (int)AudioStatusType.On;
                    RoomWindowVM.RoomClient.PublishAudio(RoomWindowVM.RoomVM.Id, Id, MicType.Public, AudioStatusType.On);
                }
            }
        }

        private bool CanPublishAudioCommandExecute(SecureCommandArgs args)
        {
            return IsMe();
        }

        public SecureCommand StopPublishAudioCommand { get; set; }

        private void StopPublishAudioCommandExecute(SecureCommandArgs args)
        {
            if (CanStopPublishAudioCommandExecute(args))
            {
                bool stopRes = this.RoomWindowVM.StopPublish();
                if (stopRes)
                {
                    AudioStatus = (int)AudioStatusType.Off;
                    RoomWindowVM.RoomClient.PublishAudio(RoomWindowVM.RoomVM.Id, Id, MicType.Public, AudioStatusType.Off);
                }
            }
        }

        private bool CanStopPublishAudioCommandExecute(SecureCommandArgs args)
        {
            return IsMe();
        }

        #endregion

        #region AgentPoralCommand

        public SecureCommand AgentPortalCommand { get; set; }

        private void AgentPortalCommandExecute(SecureCommandArgs args)
        {
            Messenger.Default.Send<EnumNotificationMessage<object, HallWindowAction>>(new EnumNotificationMessage<object, HallWindowAction>(HallWindowAction.OpenAgentPortal));
        }

        private bool CanAgentPortalCommandExecute(SecureCommandArgs args)
        {
            return true;
        }

        #endregion

        protected override void InitializeCommand()
        {
            OpenConfigurationCommand = new SecureCommand(OpenConfigurationCommandExecute, CanOpenConfigurationCommandExecute);
            SelectUserCommand = new SecureCommand(SelectUserCommandExecute, CanSelectUserCommandExecute);
            PublishAudioCommand = new SecureCommand(PublishAudioCommandExecute, CanPublishAudioCommandExecute);
            StopPublishAudioCommand = new SecureCommand(StopPublishAudioCommandExecute, CanStopPublishAudioCommandExecute);
            AgentPortalCommand = new SecureCommand(AgentPortalCommandExecute, CanAgentPortalCommandExecute);
            base.InitializeCommand();
        }
    }
}
