using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common;
using YoYoStudio.Model.Chat;

namespace YoYoStudio.Client.Chat.ValueConverters
{
    public class SpectrumAnalyzerVisibilityConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Hidden;
            int micStatus;
            if (int.TryParse(values[0].ToString(), out micStatus))
            {
                int audioStatus;
                if (int.TryParse(values[1].ToString(), out audioStatus))
                {
                    if ((micStatus & MicStatusMessage.MicStatus_Audio) != 0
                           || audioStatus == 1)
                    {
                        visibility = Visibility.Visible;
                    }
                }
            }
            return visibility;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
