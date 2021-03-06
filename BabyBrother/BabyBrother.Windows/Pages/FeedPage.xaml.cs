﻿using BabyBrother.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class FeedPage : Page
    {
        private FeedPageViewModel _viewModel;
        private CompositeDisposable _subscriptions;

        public FeedPage()
        {
            DataContext = _viewModel = App.Container.GetInstance<FeedPageViewModel>();
            this.InitializeComponent();

            _subscriptions = new CompositeDisposable();
            _subscriptions.Add(_viewModel);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _subscriptions.Dispose();
            base.OnNavigatedFrom(e);
        }

        private void OnDurationChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            _viewModel.OverrideDuration(e.NewTime);
        }
    }
}
