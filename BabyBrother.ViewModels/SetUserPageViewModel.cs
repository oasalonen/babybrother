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
    public class SetUserPageViewModel : SetItemViewModel<User>
    {
        private readonly ISubject<User> _userSelectionStream;
        private readonly ISubject<IObservable<Notification<Unit>>> _addUserStream;
        private readonly IBackendService _backendService;
        private readonly INotificationService _notificationService;
        private readonly IResourceService _resourceService;

        public ReactiveProperty<string> NewUsername { get; private set; }

        public ICommand SubmitCommand { get; private set; }

        public SetUserPageViewModel(IBackendService backendService, INotificationService notificationService, IResourceService resourceService)
        {
            _backendService = backendService;
            _notificationService = notificationService;
            _resourceService = resourceService;

            _userSelectionStream = new BehaviorSubject<User>(null);
            _addUserStream = new BehaviorSubject<IObservable<Notification<Unit>>>(Observable.Empty<Unit>().Materialize());

            NewUsername = new ReactiveProperty<string>();
            AddSubscription(NewUsername);

            var isExistingUserSelectedStream = _userSelectionStream
                .Select(user => user != null)
                .CombineLatest(CurrentState, (isUserSelected, state) => isUserSelected && state == SetByState.Existing);
            var isNewUsernameSetStream = NewUsername
                .Select(name => !string.IsNullOrWhiteSpace(name))
                .CombineLatest(CurrentState, (isNameSet, state) => isNameSet && state == SetByState.New);
            var latestAddUserStream = _addUserStream.Switch();
            var isAddUserInProgressStream = latestAddUserStream.Select(notification => notification.Kind == NotificationKind.OnNext);
            var canSubmitStream = isExistingUserSelectedStream
                .CombineLatest(isNewUsernameSetStream, (isNameSet, isUserSelected) => isNameSet || isUserSelected)
                .CombineLatest(isAddUserInProgressStream, (isUserInfoSet, isAddUserInProgress) => isUserInfoSet && !isAddUserInProgress);

            var submitCommand = new ReactiveCommand(canSubmitStream);
            AddSubscription(submitCommand.Subscribe(_ =>
            {
                OnSubmit();
            }));
            SubmitCommand = submitCommand;

            AddSubscription(latestAddUserStream
                .Where(notification => notification.Kind == NotificationKind.OnError)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(async (_) =>
                {
                    await _notificationService.ShowBlockingMessageAsync(
                        _resourceService.GetString("SetUserCreateProfileErrorMessage"),
                        _resourceService.GetString("SetUserCreateProfileErrorTitle"));
                }));

            InitializeExistingItems(_backendService.GetUsers());
        }

        public override void SelectExistingItem(User user)
        {
            _userSelectionStream.OnNext(user);
        }

        private void OnSubmit()
        {
            var newName = NewUsername.Value;
            if (!string.IsNullOrWhiteSpace(newName) && CurrentState.Value == SetByState.New)
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
