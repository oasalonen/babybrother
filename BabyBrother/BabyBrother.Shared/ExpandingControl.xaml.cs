using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace BabyBrother
{
    public sealed partial class ExpandingControl : UserControl
    {
        public enum State
        {
            Collapsed,
            Expanded
        }

        public UIElement Header
        {
            get { return (UIElement)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public UIElement Body
        {
            get { return (UIElement)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public State CurrentState
        {
            get { return (State)GetValue(CurrentStateProperty); }
            set { SetValue(CurrentStateProperty, value); }
        }

        private static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(UIElement), typeof(ExpandingControl), new PropertyMetadata(null, HeaderPropertyChanged));

        private static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(UIElement), typeof(ExpandingControl), new PropertyMetadata(null, BodyPropertyChanged));

        private static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.Register("CurrentState", typeof(State), typeof(ExpandingControl), new PropertyMetadata(State.Collapsed, CurrentStatePropertyChanged));

        private readonly ISubject<State> CurrentStateStream = new BehaviorSubject<State>(State.Collapsed);

        public ExpandingControl()
        {
            this.InitializeComponent();

            var bodyLayoutStream = Observable.FromEventPattern<object>(
                h => BodyContent.LayoutUpdated += h,
                h => BodyContent.LayoutUpdated -= h);

            var subscription = CurrentStateStream.CombineLatest(bodyLayoutStream, (state, _) => new Tuple<State, bool>(state, (state == State.Expanded && BodyContent.ActualHeight > 0) || state == State.Collapsed))
                .Where(t => t.Item2 == true)
                .Subscribe(t => UpdateSplitOpenLength(t.Item1));
        }

        private void UpdateSplitOpenLength(State state)
        {
            if (BodyContent.Content != null)
            {
                var groups = VisualStateManager.GetVisualStateGroups(RootGrid);
                var group = VisualStateManager.GetVisualStateGroups(RootGrid).First();
                var expandedVisualState = group.States.Where(g => g.Name == "Expanded");
                var animation = expandedVisualState.First().Storyboard.Children[0] as SplitOpenThemeAnimation;
                animation.OpenedLength = animation.ContentTranslationOffset = BodyContent.ActualHeight;
                VisualStateManager.GoToState(this, (state == State.Collapsed ? "Collapsed" : "Expanded"), true);
            }
        }

        private static void HeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ExpandingControl;
            self.HeaderContent.Content = e.NewValue;
        }

        private static void BodyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ExpandingControl;
            self.BodyContent.Content = e.NewValue;
        }

        private static void CurrentStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ExpandingControl;
            var state = (State) e.NewValue;
            self.ContentSection.Visibility = (state == State.Collapsed ? Visibility.Collapsed : Visibility.Visible);
            self.CurrentStateStream.OnNext(state);
        }
    }
}
