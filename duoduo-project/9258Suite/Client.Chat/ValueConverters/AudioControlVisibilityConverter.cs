using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using YoYoStudio.Client.ViewModel;

namespace YoYoStudio.Client.Chat.ValueConverters
{
    public class AudioControlVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            UserViewModel uvm = value as UserViewModel;
            var result = Visibility.Hidden;
            if (uvm != null)
            {
                if (uvm.IsMe())
                {
                    result = Visibility.Visible;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
