using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoYoStudio.Model;
using YoYoStudio.Model.Chat;

namespace YoYoStudio.ChatService.Client
{
    [Serializable]
    public class ChatServiceCache : Cache
    {
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static ChatServiceCache()
        {
            //client = new ChatServiceClient(new ChatServiceCallback());
        }

        public ChatServiceCache()
            : base(BuiltIns._9258ChatApplication.Id)
        {
        }

        public override void RefreshCache(params object[] args)
        {
            Application = BuiltIns._9258ChatApplication;
            Task.WaitAll(new Task[] { LoadExchangeRates()
                , LoadRoomGroupTask()
                , LoadGiftTask()
                , LoadRoleCommandTask()
                , LoadImageTask()
                , LoadRoomRoleTask()
                , LoadTask()
            });
            base.RefreshCache(args);
            logger.Error("Refresh chat service succeed");
        }

        private Task LoadExchangeRates()
        {
            return Task.Factory.StartNew(() =>
                {
                    ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                    ExchangeRates = client.GetExchangeRates().ToList();
                    client.Close();
                });
        }

        private Task LoadRoomGroupTask()
        {
            return Task.Factory.StartNew(() =>
                {
                    ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                    RoomGroups = client.GetRoomGroups().ToList();
                    Rooms = client.GetRooms().Where(r=>r.RoomGroup_Id.HasValue && r.RoomGroup_Id.Value >0).ToList();
                    client.Close();
                });
        }

        private Task LoadGiftTask()
        {
            return Task.Factory.StartNew(() =>
                {
                    ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                    GiftGroups = client.GetGiftGroups().ToList();
                    Gifts = client.GetGifts().ToList();
                    client.Close();
                });
        }

        private Task LoadRoleCommandTask()
        {
            return Task.Factory.StartNew(() =>
            {
                ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                Roles = client.GetRoles().ToList();
                RoleCommands = client.GetRoleCommands().ToList();
                client.Close();
            });
        }

        private Task LoadImageTask()
        {
            return Task.Factory.StartNew(() =>
                {
                    ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                    int imgCount = client.GetImageCount();
					Images = new List<Model.Core.ImageWithoutBody>();
                    if (imgCount > 0)
                    {
                        int batch = imgCount / countPerBatch;
                        int left = imgCount % countPerBatch;
                        int start = 1;
                        int count = countPerBatch;
                        for (int i = 0; i < batch; i++)
                        {
                            Images.AddRange(client.GetImages(start, count));
                            start = start + countPerBatch;
                        }
                        if (left > 0)
                        {
                            Images.AddRange(client.GetImages(start, left));
                        }
                    }
                    client.Close();
                });
        }

        private Task LoadRoomRoleTask()
        {
            return Task.Factory.StartNew(() =>
                {
                    ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                    int roomRoleCount = client.GetRoomRoleCount();
					RoomRoles = new List<Model.Chat.RoomRole>();
                    if (roomRoleCount > 0)
                    {                        
                        int batch = roomRoleCount / countPerBatch;
                        int left = roomRoleCount % countPerBatch;
                        int start = 1;
                        int count = countPerBatch;
                        for (int i = 0; i < batch; i++)
                        {
                            RoomRoles.AddRange(client.GetRoomRoles(start, count));
                            start = start + countPerBatch;
                        }
                        if (left > 0)
                        {
                            RoomRoles.AddRange(client.GetRoomRoles(start, left));
                        }
                    }
                    client.Close();
                });
        }

        private Task LoadTask()
        {
            return Task.Factory.StartNew(() =>
            {
                ChatServiceClient client = new ChatServiceClient(new ChatServiceCallback());
                Commands = client.GetCommands().ToList();
                BlockTypes = client.GetBlockTypes().ToList();
                client.Close();
            });
        }

        public override bool HasCommand(int roomId, int cmdId, int sourceUserId, int sourceRoleId, int targetRoleId)
        {
            var roleCmd = GetRoleCommand(roomId, cmdId, sourceUserId, sourceRoleId, targetRoleId);
            if (roleCmd != null)
            {
                if (roleCmd.IsManagerCommand)
                {
                    return IsUserInRoomRole(roomId, sourceUserId, BuiltIns._9258RoomAdministratorRole.Id);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }

    public partial class ChatServiceClient
    {
        static ChatServiceClient()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                });
        }

        public ChatServiceClient(IChatServiceCallback callback) :
            this(new System.ServiceModel.InstanceContext(callback), Const.ChatServiceName)
        {
        }
    }
}

