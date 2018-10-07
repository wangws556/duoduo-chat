using Snippets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoYoStudio.Common;

namespace YoYoStudio.Client.ViewModel
{
    [SnippetPropertyINPC(field = "audioRtmpUrl", property = "AudioRtmpUrl", type = "string", defaultValue = "string.Empty")]
    public partial class AudioWindowViewModel:WindowViewModel
    {
        public RoomWindowViewModel RoomWindowVM { get { return Singleton<ApplicationViewModel>.Instance.RoomWindowVM; } }

        public AudioWindowViewModel():base()
        {
            AudioRtmpUrl = "rtmp://" + ConfigurationManager.AppSettings["AudioServiceIp"] + "/live";
        }
    }
}
