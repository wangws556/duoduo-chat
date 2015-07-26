using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YoYoStudio.Client.ViewModel;
using YoYoStudio.Common;

namespace YoYoStudio.Client.Chat.ValueConverters
{
    public class SpectrumAnalyzerVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null)
            {
                int Id = (int)value;
                if(Id == Singleton<ApplicationViewModel>.Instance.LocalCache.CurrentUserVM.Id)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
