﻿using BabyBrother.Base;
using BabyBrother.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ReactiveProperty<TimeSpan> Duration { get; private set; }

        public ReactiveProperty<bool> IsRunning { get; private set; }

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

            var isStartTimeSetStream = StartTime.Select(time => TimeUtilities.IsValid(time));
            var start = new ReactiveCommand(isStartTimeSetStream.Select(isSet => !isSet));
            Start = start;
            AddSubscription(start);
            AddSubscription(start.Subscribe(_ => StartTime.Value = DateTimeOffset.Now));

            var isStopTimeSetStream = StopTime.Select(time => TimeUtilities.IsValid(time));
            var canStopStream = isStartTimeSetStream.CombineLatest(isStopTimeSetStream, (isStartTimeSet, isStopTimeSet) => isStartTimeSet && !isStopTimeSet);
            var stop = new ReactiveCommand(canStopStream);
            Stop = stop;
            AddSubscription(stop);
            AddSubscription(stop.Subscribe(_ => StopTime.Value = DateTimeOffset.Now));

            IsRunning = canStopStream.ToReactiveProperty();
            AddSubscription(IsRunning);

            var heartbeat = Observable
                .Interval(TimeSpan.FromMilliseconds(500))
                .Select(_ => DateTimeOffset.Now);

            var validStartTimeStream = StartTime.Where(time => TimeUtilities.IsValid(time));

            Duration = IsRunning
                .Select(isRunning => isRunning ? heartbeat : StopTime)
                .Switch()
                .Where(time => TimeUtilities.IsValid(time))
                .CombineLatest(validStartTimeStream, (stopTime, startTime) => stopTime - startTime)
                .ToReactiveProperty();
            AddSubscription(Duration);

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

            AddSubscription(toggleRight.Subscribe(_ =>
            {
                IsRightBreastSelected.Value = !IsRightBreastSelected.Value;
                if (IsRightBreastSelected.Value)
                {
                    IsBottleSelected.Value = false;
                }
            }));
            AddSubscription(toggleLeft.Subscribe(_ =>
            {
                IsLeftBreastSelected.Value = !IsLeftBreastSelected.Value;
                if (IsLeftBreastSelected.Value)
                {
                    IsBottleSelected.Value = false;
                }
            }));
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

        public void OverrideDuration(TimeSpan timeSpan)
        {
            if (TimeUtilities.IsValid(StartTime.Value) && TimeUtilities.IsValid(StopTime.Value))
            {
                StopTime.Value = StartTime.Value + timeSpan;
            }
        }
    }
}
