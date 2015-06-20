using BabyBrother.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BabyBrother.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Boolean booleanValue =
                (value is Boolean ?
                (Boolean)value :    // type is bool, use it
                (value is DateTimeOffset ?
                TimeUtilities.IsValid((DateTimeOffset)value) :
                (value != null)));   // type is something else, use null check

            if (IsInverted)
            {
                return booleanValue ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return booleanValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
