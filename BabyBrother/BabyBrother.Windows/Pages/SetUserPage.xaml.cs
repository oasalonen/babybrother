using BabyBrother.Models;
using BabyBrother.Services.Implementations;
using BabyBrother.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

namespace BabyBrother.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetUserPage : Page
    {
        private SetUserPageViewModel _viewModel;
        private CompositeDisposable _subscriptions;

        public SetUserPage()
        {
            DataContext = _viewModel = App.Container.GetInstance<SetUserPageViewModel>();
            this.InitializeComponent();

            _subscriptions = new CompositeDisposable();
            _subscriptions.Add(_viewModel.CurrentState
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(state =>
                {
                    NewUserSection.CurrentState = (state == SetByState.New ? ExpandingControl.State.Expanded : ExpandingControl.State.Collapsed);
                    ExistingUserSection.CurrentState = (state == SetByState.Existing ? ExpandingControl.State.Expanded : ExpandingControl.State.Collapsed);
                }));

            _subscriptions.Add(_viewModel.IsSubmitting
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(isSubmitting =>
                {
                    VisualStateManager.GoToState(this, isSubmitting ? "Submitting" : "NotSubmitting", false);
                }));

            _subscriptions.Add(_viewModel.ActionStream
                .Where(a => a == SetUserPageViewModel.RequestedAction.Complete)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => 
                {
                    Frame.Navigate(typeof(SetInfantPage));
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
            _viewModel.SelectExistingItem(e.AddedItems.FirstOrDefault() as User);
        }
    }
}
