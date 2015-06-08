using BabyBrother.Models;
using BabyBrother.Services.Implementations;
using BabyBrother.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BabyBrother
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SetUserPageViewModel _viewModel;
        private CompositeDisposable _subscriptions;

        public MainPage()
        {
            DataContext = _viewModel = App.Container.GetInstance<SetUserPageViewModel>();
            this.InitializeComponent();

            _subscriptions = new CompositeDisposable();
            _subscriptions.Add(_viewModel.CurrentState.Subscribe((state) =>
            {
                NewUserSection.CurrentState = (state == SetUserPageViewModel.State.SetByNew ? ExpandingControl.State.Expanded : ExpandingControl.State.Collapsed);
                ExistingUserSection.CurrentState = (state == SetUserPageViewModel.State.SetByExisting ? ExpandingControl.State.Expanded : ExpandingControl.State.Collapsed);
            }));
            _subscriptions.Add(_viewModel);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _subscriptions.Dispose();
            base.OnNavigatedFrom(e);
        }

        private void OnExistingUserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectExistingUser(e.AddedItems.FirstOrDefault() as User);
        }
    }

    public class SetByButtonSelectionConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (SetUserPageViewModel.State) value;
            return IsInverted ? state != SetUserPageViewModel.State.SetByNew : state == SetUserPageViewModel.State.SetByNew;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var isSelected = (bool)value;
            return IsInverted ? SetUserPageViewModel.State.SetByExisting : SetUserPageViewModel.State.SetByNew;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Boolean booleanValue = 
                (value is Boolean ? 
                (Boolean) value :      // type is bool, use it
                (value != null));   // type is something else, use null check
            
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
