using BabyBrother.Models;
using BabyBrother.Services;
using BabyBrother.ViewModels.Common;
using Reactive.Bindings;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;

namespace BabyBrother.ViewModels
{
    public class SetUserPageViewModel : IDisposable
    {
        private readonly CompositeDisposable _subscriptions;
        private readonly ISubject<User> _userSelectionStream;
        private readonly ISubject<IObservable<Notification<Unit>>> _addUserStream;
        private readonly IBackendService _backendService;
        private readonly INotificationService _notificationService;
        private readonly IResourceService _resourceService;

        public enum State
        {
            SetByNew,
            SetByExisting
        }

        public ReactiveProperty<State> CurrentState { get; private set; }

        public ReactiveProperty<string> NewUsername { get; private set; }

        public ReactiveProperty<bool> IsExistingUsersAvailable { get; private set; }

        public ReactiveProperty<LoadState> ExistingUsersLoadState { get; private set; }

        // TODO: make readonly
        public ReactiveCollection<User> ExistingUsers { get; private set; }
        
        public ICommand SetByNewCommand { get; private set; }

        public ICommand SetByExistingCommand { get; private set; }

        public ICommand SubmitCommand { get; private set; }

        public SetUserPageViewModel(IBackendService backendService, INotificationService notificationService, IResourceService resourceService)
        {
            _backendService = backendService;
            _notificationService = notificationService;
            _resourceService = resourceService;

            _subscriptions = new CompositeDisposable();
            _userSelectionStream = new BehaviorSubject<User>(null);
            _addUserStream = new BehaviorSubject<IObservable<Notification<Unit>>>(Observable.Empty<Unit>().Materialize());
            var setUserByNew = new ReactiveCommand();
            var setUserByExisting = new ReactiveCommand();

            SetByNewCommand = setUserByNew;
            SetByExistingCommand = setUserByExisting;

            var setByNewStream = setUserByNew.Select(_ => State.SetByNew);
            var setByExistingStream = setUserByExisting.Select(_ => State.SetByExisting);

            CurrentState = setByNewStream.Merge(setByExistingStream)
                .ToReactiveProperty(State.SetByNew);
            _subscriptions.Add(CurrentState);

            NewUsername = new ReactiveProperty<string>();
            _subscriptions.Add(NewUsername);

            var userStream = _backendService.GetUsers();
            var safeUserStream = userStream.Catch(Observable.Empty<User>());
            ExistingUsers = safeUserStream.ToReactiveCollection();
            _subscriptions.Add(ExistingUsers);

            IsExistingUsersAvailable = safeUserStream.Any().ToReactiveProperty();
            _subscriptions.Add(IsExistingUsersAvailable);

            var isExistingUserSelectedStream = _userSelectionStream
                .Select(user => user != null)
                .CombineLatest(CurrentState, (isUserSelected, state) => isUserSelected && state == State.SetByExisting);
            var isNewUsernameSetStream = NewUsername
                .Select(name => !string.IsNullOrWhiteSpace(name))
                .CombineLatest(CurrentState, (isNameSet, state) => isNameSet && state == State.SetByNew);
            var latestAddUserStream = _addUserStream.Switch();
            var isAddUserInProgressStream = latestAddUserStream.Select(notification => notification.Kind == NotificationKind.OnNext);
            var canSubmitStream = isExistingUserSelectedStream
                .CombineLatest(isNewUsernameSetStream, (isNameSet, isUserSelected) => isNameSet || isUserSelected)
                .CombineLatest(isAddUserInProgressStream, (isUserInfoSet, isAddUserInProgress) => isUserInfoSet && !isAddUserInProgress);

            var submitCommand = new ReactiveCommand(canSubmitStream);
            _subscriptions.Add(submitCommand.Subscribe(_ =>
            {
                OnSubmit();
            }));
            SubmitCommand = submitCommand;

            _subscriptions.Add(latestAddUserStream
                .Where(notification => notification.Kind == NotificationKind.OnError)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(async (_) =>
                {
                    await _notificationService.ShowBlockingMessageAsync(
                        _resourceService.GetString("SetUserCreateProfileErrorMessage"),
                        _resourceService.GetString("SetUserCreateProfileErrorTitle"));
                }));

            ExistingUsersLoadState = userStream.Materialize()
                .Select(notification => notification.Kind == NotificationKind.OnError ? LoadState.LoadedError : LoadState.Loaded)
                .CombineLatest(userStream.Any(), (loadState, isAny) => 
                {
                    return loadState == LoadState.LoadedError ? 
                        LoadState.LoadedError : 
                        (isAny ? LoadState.Loaded : LoadState.LoadedEmpty);
                })
                .StartWith(LoadState.Loading)
                .Catch(Observable.Return(LoadState.LoadedError))
                .ToReactiveProperty();
            _subscriptions.Add(ExistingUsersLoadState);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        public void SelectExistingUser(User user)
        {
            _userSelectionStream.OnNext(user);
        }

        private void OnSubmit()
        {
            var newName = NewUsername.Value;
            if (!string.IsNullOrWhiteSpace(newName) && CurrentState.Value == State.SetByNew)
            {
                var user = new User
                {
                    Name = newName
                };
                _addUserStream.OnNext(_backendService
                    .AddUser(user)
                    .Materialize()
                    .Catch(Observable.Empty<Notification<Unit>>()));
            }
        }
    }
}
