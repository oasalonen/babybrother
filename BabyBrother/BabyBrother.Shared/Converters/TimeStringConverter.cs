using BabyBrother.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml.Data;

namespace BabyBrother.Converters
{
    public class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset)
            {
                var dateTime = (DateTimeOffset)value;
                if (TimeUtilities.IsValid(dateTime))
                {
                    return dateTime.ToString("g", CultureInfo.CurrentUICulture);
                }
                else
                {
                    return "-";
                }
            }
            else if (value is TimeSpan)
            {
                var timeSpan = (TimeSpan)value;
                return timeSpan.ToString("hh\\:mm\\:ss");
            }
            else
            {
                return "-";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
