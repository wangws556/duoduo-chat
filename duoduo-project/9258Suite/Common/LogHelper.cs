using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoYoStudio.Common
{
    public class LogHelper
    {
        public static readonly ILog InfoLogger = LogManager.GetLogger("loginfo");
        public static readonly ILog ErrorLogger = LogManager.GetLogger("logerror");
    }
    public class LogHelperRtmp
    {
        public static readonly ILog InfoLogger = LogManager.GetLogger("loginfortmp");
        public static readonly ILog ErrorLogger = LogManager.GetLogger("logerrorrtmp");
    }
}
