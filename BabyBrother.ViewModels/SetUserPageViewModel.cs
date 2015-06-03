using BabyBrother.Base;
using BabyBrother.Models;
using BabyBrother.Services;
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
        private readonly ISubject<User> _userSelectionStream;
        private readonly AzureBackendService _backendService;

        public enum State
        {
            SetByNew,
            SetByExisting
        }

        public ReactiveProperty<State> CurrentState { get; private set; }

        public ReactiveProperty<string> NewUsername { get; private set; }

        public ReactiveProperty<bool> IsExistingUsersAvailable { get; private set; }

        // TODO: make readonly
        public ReactiveCollection<User> ExistingUsers { get; private set; }
        
        public ICommand SetByNewCommand { get; private set; }

        public ICommand SetByExistingCommand { get; private set; }

        public ICommand SubmitCommand { get; private set; }

        public SetUserPageViewModel()
        {
            _backendService = new AzureBackendService();
            _subscriptions = new CompositeDisposable();
            _userSelectionStream = new BehaviorSubject<User>(null);
            var setUserByNew = new ReactiveCommand();
            var setUserByExisting = new ReactiveCommand();

            SetByNewCommand = setUserByNew;
            SetByExistingCommand = setUserByExisting;

            var setByNewStream = setUserByNew.Select(_ => State.SetByNew);
            var setByExistingStream = setUserByExisting.Select(_ => State.SetByExisting);

            CurrentState = setByNewStream.Merge(setByExistingStream)
                .Do(state => System.Diagnostics.Debug.WriteLine("STATE " + state.ToString()))
                .ToReactiveProperty(State.SetByNew);
            _subscriptions.Add(CurrentState);

            NewUsername = new ReactiveProperty<string>();
            _subscriptions.Add(NewUsername);

            var userStream = _backendService.GetUsers()
                .SelectMany(users => users);
            ExistingUsers = userStream.ToReactiveCollection();
            _subscriptions.Add(ExistingUsers);

            IsExistingUsersAvailable = userStream.Any().ToReactiveProperty();
            _subscriptions.Add(IsExistingUsersAvailable);

            var canSubmitStream = _userSelectionStream.Select(user => user != null)
                .CombineLatest(NewUsername, (isUserSelected, name) => isUserSelected || !string.IsNullOrWhiteSpace(name));

            var submitCommand = new ReactiveCommand(canSubmitStream);
            _subscriptions.Add(submitCommand.Subscribe(_ =>
            {
                OnSubmit();
            }));
            SubmitCommand = submitCommand;
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        public void SelectExistingUser(User user)
        {
            _userSelectionStream.OnNext(user);
        }

        private async void OnSubmit()
        {
            var user = new User
            {
                Name = NewUsername.Value
            };
            await _backendService.AddUser(user);
        }

        private void LoadExistingUsers()
        {

        }
    }
}
