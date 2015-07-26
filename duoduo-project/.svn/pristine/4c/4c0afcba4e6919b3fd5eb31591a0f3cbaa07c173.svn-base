using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YoYoStudio.Client.ViewModel;

namespace YoYoStudio.Client.Chat.ValueConverters
{
    public class BoolToVisibleAndHiddenConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool tag = (bool)value;
            if (tag == true)
            {
                return System.Windows.Visibility.Visible;
            }
            else
                return System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Visibility v = (System.Windows.Visibility)value;
            if (v == System.Windows.Visibility.Visible)
                return true;
            else
                return false;
        }
    }

    public class BoolToVisibleAndCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool tag = (bool)value;
            if (tag == true)
            {
                return System.Windows.Visibility.Visible;
            }
            else
                return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Visibility v = (System.Windows.Visibility)value;
            if (v == System.Windows.Visibility.Visible)
                return true;
            else
                return false;
        }
    }

    public class NegativeBoolToVisibleAndHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool tag = (bool)value;
            if (tag == false)
            {
                return System.Windows.Visibility.Visible;
            }
            else
                return System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Visibility v = (System.Windows.Visibility)value;
            if (v == System.Windows.Visibility.Visible)
                return false;
            else
                return false;
        }
    }
}
