﻿using BabyBrother.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel;
        private CompositeDisposable _subscriptions;

        public MainPage()
        {
            DataContext = _viewModel = App.Container.GetInstance<MainPageViewModel>();
            this.InitializeComponent();

            _subscriptions = new CompositeDisposable();

            _subscriptions.Add(_viewModel.ActionStream
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(action =>
                {
                    switch (action)
                    {
                        case MainPageViewModel.RequestedAction.GoFeed:
                            Frame.Navigate(typeof(FeedPage));
                            break;
                        default:
                            break;
                    }
                }));

            _subscriptions.Add(_viewModel);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _subscriptions.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
