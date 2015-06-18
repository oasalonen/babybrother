using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BabyBrother.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        public enum RequestedAction
        {
            GoFeed,
            GoChangeDiapers,
            GoSleep,
            GoMeasure
        }

        public IObservable<RequestedAction> ActionStream { get; private set; }

        public ICommand Feed { get; private set; }

        public ICommand ChangeDiapers { get; private set; }

        public ICommand Sleep { get; private set; }

        public ICommand Measure { get; private set; }

        public MainPageViewModel()
        {
            var feed = new ReactiveCommand();
            var changeDiapers = new ReactiveCommand();
            var sleep = new ReactiveCommand();
            var measure = new ReactiveCommand();
            Feed = feed;
            ChangeDiapers = changeDiapers;
            Sleep = sleep;
            Measure = measure;
            AddSubscription(feed);
            AddSubscription(changeDiapers);
            AddSubscription(sleep);
            AddSubscription(measure);

            var feedStream = feed.Select(_ => RequestedAction.GoFeed);
            var changeDiapersStream = changeDiapers.Select(_ => RequestedAction.GoChangeDiapers);
            var sleepStream = sleep.Select(_ => RequestedAction.GoSleep);
            var measureStream = measure.Select(_ => RequestedAction.GoMeasure);
            ActionStream = feedStream
                .Merge(changeDiapersStream)
                .Merge(sleepStream)
                .Merge(measureStream);
        }
    }
}
