using BabyBrother.Models;
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
    public class FeedPageViewModel : ViewModel
    {
        public IObservable<Feeding.Source> Source { get; private set; }

        public ReactiveProperty<DateTimeOffset> StartTime { get; private set; }

        public ReactiveProperty<DateTimeOffset> StopTime { get; private set; }

        public ReactiveProperty<bool> IsLeftBreastSelected { get; private set; }

        public ReactiveProperty<bool> IsRightBreastSelected { get; private set; }

        public ReactiveProperty<bool> IsBottleSelected { get; private set; }

        public ICommand ToggleLeftBreast { get; private set; }

        public ICommand ToggleRightBreast { get; private set; }

        public ICommand ToggleBottle { get; private set; }

        public ICommand Start { get; private set; }

        public ICommand Stop { get; private set; }

        public FeedPageViewModel()
        {
            StartTime = new ReactiveProperty<DateTimeOffset>(DateTimeOffset.MinValue);
            StopTime = new ReactiveProperty<DateTimeOffset>(DateTimeOffset.MinValue);

            var isStartTimeSetStream = StartTime.Select(time => IsValidDateTime(time));
            var start = new ReactiveCommand(isStartTimeSetStream.Select(isSet => !isSet));
            Start = start;
            AddSubscription(start);

            var isStopTimeSetStream = StopTime.Select(time => IsValidDateTime(time));
            var canStopStream = isStartTimeSetStream.CombineLatest(isStopTimeSetStream, (isStartTimeSet, isStopTimeSet) => isStartTimeSet && !isStopTimeSet);
            var stop = new ReactiveCommand(canStopStream);
            Stop = stop;
            AddSubscription(stop);

            IsLeftBreastSelected = new ReactiveProperty<bool>(false);
            IsRightBreastSelected = new ReactiveProperty<bool>(false);
            IsBottleSelected = new ReactiveProperty<bool>(false);
            AddSubscription(IsLeftBreastSelected);
            AddSubscription(IsRightBreastSelected);
            AddSubscription(IsBottleSelected);

            Source = IsLeftBreastSelected
                .CombineLatest(IsRightBreastSelected, (isLeft, isRight) =>
                {
                    if (isLeft && isRight)
                    {
                        return Feeding.Source.LeftRight;
                    }
                    else if (isRight)
                    {
                        return Feeding.Source.Right;
                    }
                    else if (isLeft)
                    {
                        return Feeding.Source.Left;
                    }
                    else
                    {
                        return Feeding.Source.None;
                    }
                })
                .CombineLatest(IsBottleSelected, (breastSource, isBottle) =>
                {
                    return isBottle ? Feeding.Source.Bottle : breastSource;
                });

            var toggleRight = new ReactiveCommand();
            var toggleLeft = new ReactiveCommand();
            var toggleBottle = new ReactiveCommand();
            ToggleRightBreast = toggleRight;
            ToggleLeftBreast = toggleLeft;
            ToggleBottle = toggleBottle;
            AddSubscription(toggleRight);
            AddSubscription(toggleLeft);
            AddSubscription(toggleBottle);

            AddSubscription(toggleRight.Subscribe(_ => IsRightBreastSelected.Value = !IsRightBreastSelected.Value));
            AddSubscription(toggleLeft.Subscribe(_ => IsLeftBreastSelected.Value = !IsLeftBreastSelected.Value));
            AddSubscription(toggleBottle.Subscribe(_ => 
            {
                IsBottleSelected.Value = !IsBottleSelected.Value;
                if (IsBottleSelected.Value)
                {
                    IsRightBreastSelected.Value = false;
                    IsLeftBreastSelected.Value = false;
                }
            }));
        }

        private static bool IsValidDateTime(DateTimeOffset? time)
        {
            return !(time == null || time.Value == DateTimeOffset.MinValue || time.Value == new DateTimeOffset());
        }
    }
}
