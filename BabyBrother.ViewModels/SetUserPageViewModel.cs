using BabyBrother.Base;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Input;

namespace BabyBrother.ViewModels
{
    public class SetUserPageViewModel : IDisposable
    {
        private readonly CompositeDisposable _subscriptions;

        public enum State
        {
            SetByNew,
            SetByExisting
        }

        public ReactiveProperty<State> CurrentState { get; private set; }
        
        public ICommand SetByNewCommand { get; private set; }

        public ICommand SetByExistingCommand { get; private set; }

        public ICommand SubmitCommand { get; private set; }

        public SetUserPageViewModel()
        {
            _subscriptions = new CompositeDisposable();
            var setUserByNew = new ReactiveCommand();
            var setUserByExisting = new ReactiveCommand();
            var submitCommand = new ReactiveCommand();

            SetByNewCommand = setUserByNew;
            SetByExistingCommand = setUserByExisting;

            var setByNewStream = setUserByNew.Select(_ => State.SetByNew);
            var setByExistingStream = setUserByExisting.Select(_ => State.SetByExisting);

            CurrentState = setByNewStream.Merge(setByExistingStream)
                .Do(state => System.Diagnostics.Debug.WriteLine("STATE " + state.ToString()))
                .ToReactiveProperty(State.SetByNew);

            _subscriptions.Add(CurrentState);

            submitCommand.Subscribe(_ =>
            {
            });
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }
    }
}
