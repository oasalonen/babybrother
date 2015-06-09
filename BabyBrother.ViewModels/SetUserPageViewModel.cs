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
        private readonly IBackendService _backendService;
        private readonly INotificationService _notificationService;
        private readonly IResourceService _resourceService;

        public ReactiveProperty<string> NewUsername { get; private set; }

        public SetUserPageViewModel(IBackendService backendService, INotificationService notificationService, IResourceService resourceService)
        {
            _backendService = backendService;
            _notificationService = notificationService;
            _resourceService = resourceService;
            _userSelectionStream = new BehaviorSubject<User>(null);
            
            NewUsername = new ReactiveProperty<string>();
            AddSubscription(NewUsername);

            InitializeExistingItems(_backendService.GetUsers());
            InitializeSubmit();
        }

        public override void SelectExistingItem(User user)
        {
            _userSelectionStream.OnNext(user);
        }

        protected override IObservable<bool> IsReadyToSubmit()
        {
            var isExistingUserSelectedStream = _userSelectionStream
                .Select(user => user != null)
                .CombineLatest(CurrentState, (isUserSelected, state) => isUserSelected && state == SetByState.Existing);
            var isNewUsernameSetStream = NewUsername
                .Select(name => !string.IsNullOrWhiteSpace(name))
                .CombineLatest(CurrentState, (isNameSet, state) => isNameSet && state == SetByState.New);
            var canSubmitStream = isExistingUserSelectedStream
                .CombineLatest(isNewUsernameSetStream, (isNameSet, isUserSelected) => isNameSet || isUserSelected);
            
            return canSubmitStream;
        }

        protected override IObservable<Unit> OnSubmit()
        {
            var newName = NewUsername.Value;
            if (!string.IsNullOrWhiteSpace(newName) && CurrentState.Value == SetByState.New)
            {
                var user = new User
                {
                    Name = newName
                };
                return _backendService.AddUser(user);
            }
            else
            {
                return Observable.Empty<Unit>();
            }
        }

        protected override async void OnSubmitError()
        {
            await _notificationService.ShowBlockingMessageAsync(
                _resourceService.GetString("SetUserCreateProfileErrorMessage"),
                _resourceService.GetString("SetUserCreateProfileErrorTitle"));
        }
    }
}
