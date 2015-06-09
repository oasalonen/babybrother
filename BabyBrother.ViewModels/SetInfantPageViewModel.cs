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
        private readonly IResourceService _resourceService;

        public ReactiveProperty<string> Name { get; private set; }

        public ReactiveProperty<DateTime> DateOfBirth { get; private set; }

        public ReactiveProperty<GenderItem> Gender { get; private set; }

        public ICollection<GenderItem> AvailableGenders { get; private set; }

        public SetInfantPageViewModel(IBackendService backendService, IResourceService resourceService)
        {
            _backendService = backendService;
            _resourceService = resourceService;

            Name = new ReactiveProperty<string>();
            AddSubscription(Name);

            DateOfBirth = new ReactiveProperty<DateTime>(DateTime.Now);

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

            Gender = new ReactiveProperty<GenderItem>(AvailableGenders.First());
            AddSubscription(Gender);

            InitializeExistingItems(_backendService.GetInfants());
            InitializeSubmit();
        }

        protected override IObservable<Unit> OnSubmit()
        {
            throw new NotImplementedException();
        }

        protected override void OnSubmitError()
        {
            throw new NotImplementedException();
        }

        protected override IObservable<bool> IsReadyToSubmit()
        {
            var isNameSetStream = Name.Select(name => !string.IsNullOrWhiteSpace(name));
            return CurrentState
                .Select(state => state == SetByState.New)
                .CombineLatest(isNameSetStream, (isSetNew, isNameSet) => isSetNew && isNameSet);
        }
    }
}
