using BabyBrother.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace BabyBrother.Converters
{
    public class SetByButtonSelectionConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (SetUserPageViewModel.State)value;
            return IsInverted ? state != SetUserPageViewModel.State.SetByNew : state == SetUserPageViewModel.State.SetByNew;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var isSelected = (bool)value;
            return IsInverted ? SetUserPageViewModel.State.SetByExisting : SetUserPageViewModel.State.SetByNew;
        }
    }
}
