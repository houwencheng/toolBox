using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls.TimeSpanSet.Controls
{
    public class ValueToTimeConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (double)value;
            var date = DateTime.Parse("00:00:00");
            return date.AddMinutes(data).ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var para= (System.Windows.Data.RelativeSource)parameter;
            var dateString = (string)value;
            var date = new DateTime();
            var ok = DateTime.TryParse(dateString, out date);
            if (!ok) return 0.0;
            var hour = date.Hour;
            var minute = date.Minute;
            var allMinute = hour * 60 + minute;
            return allMinute;
        }
    }
}
