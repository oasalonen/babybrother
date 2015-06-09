using BabyBrother.Models;
using BabyBrother.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.ViewModels
{
    public class SetInfantPageViewModel : SetItemViewModel<Infant>
    {
        private readonly IBackendService _backendService;

        public SetInfantPageViewModel(IBackendService backendService)
        {
            _backendService = backendService;

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
            throw new NotImplementedException();
        }
    }
}
