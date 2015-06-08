using BabyBrother.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BabyBrother.Converters
{
    public class LoadStateToVisibilityConverter : IValueConverter
    {
        public bool IsLoadingControl { get; set; }

        public bool IsLoadedErrorControl { get; set; }

        public bool IsLoadedEmptyControl { get; set; }

        public bool IsLoadedDataControl { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (LoadState)value;
            bool isVisible = IsLoadingControl && state == LoadState.Loading;
            isVisible = isVisible || IsLoadedErrorControl && state == LoadState.LoadedError;
            isVisible = isVisible || IsLoadedEmptyControl && state == LoadState.LoadedEmpty;
            isVisible = isVisible || IsLoadedDataControl && state == LoadState.Loaded;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
