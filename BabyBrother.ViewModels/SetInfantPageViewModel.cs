using BabyBrother.Models;
using BabyBrother.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.ViewModels
{
    public class SetInfantPageViewModel : SetItemViewModel<Infant>
    {
        public class GenderItem
        {
            public string DisplayName { get; set; }
            public Infant.GenderType Gender { get; set; }
        }

        private readonly IBackendService _backendService;
        private readonly INotificationService _notificationService;
        private readonly IResourceService _resourceService;

        public ReactiveProperty<string> Name { get; private set; }

        public ReactiveProperty<DateTimeOffset> DateOfBirth { get; private set; }

        public ReactiveProperty<GenderItem> Gender { get; private set; }

        public ICollection<GenderItem> AvailableGenders { get; private set; }

        public SetInfantPageViewModel(IBackendService backendService, INotificationService notificationService, IResourceService resourceService)
        {
            _backendService = backendService;
            _notificationService = notificationService;
            _resourceService = resourceService;

            Name = new ReactiveProperty<string>();
            AddSubscription(Name);

            DateOfBirth = new ReactiveProperty<DateTimeOffset>(DateTimeOffset.Now);

            AvailableGenders = new List<GenderItem>
            {
                new GenderItem 
                {
                    DisplayName = _resourceService.GetString("GenderMale"),
                    Gender = Infant.GenderType.Male
                },
                new GenderItem 
                {
                    DisplayName = _resourceService.GetString("GenderFemale"),
                    Gender = Infant.GenderType.Female
                },
                new GenderItem 
                {
                    DisplayName = _resourceService.GetString("GenderOther"),
                    Gender = Infant.GenderType.Other
                }
            };

            Gender = new ReactiveProperty<GenderItem>();
            AddSubscription(Gender);

            InitializeExistingItems(_backendService.GetInfants());
            InitializeSubmit();
        }

        protected override IObservable<Unit> OnSubmit()
        {
            var name = Name.Value;
            if (!string.IsNullOrWhiteSpace(name) && 
                Gender.Value != null &&
                CurrentState.Value == SetByState.New)
            {
                var infant = new Infant
                {
                    Name = name,
                    Gender = Gender.Value.Gender.ToString(),
                    BirthDate = DateOfBirth.Value
                };
                return _backendService.AddInfant(infant);
            }
            else
            {
                return Observable.Empty<Unit>();
            }
        }

        protected override async void OnSubmitError()
        {
            await _notificationService.ShowBlockingMessageAsync(
                _resourceService.GetString("SetInfantCreateProfileErrorMessage"),
                _resourceService.GetString("SetInfantCreateProfileErrorTitle"));
        }

        protected override IObservable<bool> IsReadyToSubmit()
        {
            var isNameSetStream = Name.Select(name => !string.IsNullOrWhiteSpace(name));
            var isGenderSetStream = Gender.Select(gender => gender != null);
            return CurrentState
                .Select(state => state == SetByState.New)
                .CombineLatest(isNameSetStream, (isSetNew, isNameSet) => isSetNew && isNameSet)
                .CombineLatest(isGenderSetStream, (isInfoSet, isGenderSet) => isInfoSet && isGenderSet);
        }
    }
}
