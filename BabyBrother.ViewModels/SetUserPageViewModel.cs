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
        private readonly IBackendService _backendService;
        private readonly INotificationService _notificationService;
        private readonly IResourceService _resourceService;

        public ReactiveProperty<string> NewUsername { get; private set; }

        public SetUserPageViewModel(IBackendService backendService, INotificationService notificationService, IResourceService resourceService)
        {
            _backendService = backendService;
            _notificationService = notificationService;
            _resourceService = resourceService;
            
            NewUsername = new ReactiveProperty<string>();
            AddSubscription(NewUsername);

            InitializeExistingItems(_backendService.GetUsers());
            InitializeSubmit();
        }

        protected override IObservable<bool> IsReadyToSubmit()
        {
            var canSubmitStream = NewUsername
                .Select(name => !string.IsNullOrWhiteSpace(name))
                .CombineLatest(CurrentState, (isNameSet, state) => isNameSet && state == SetByState.New);
            
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
