using BabyBrother.Models;
using BabyBrother.Services;
using BabyBrother.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace BabyBrother.UnitTest
{
    [TestClass]
    public class TestSetInfantPageViewModel
    {
        private IBackendService _backendService;
        private INotificationService _notificationService;
        private IResourceService _resourceLoader;
        private SetInfantPageViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            _backendService = Mock.Create<IBackendService>();
            _notificationService = Mock.Create<INotificationService>();
            _resourceLoader = Mock.Create<IResourceService>();
            _viewModel = new SetInfantPageViewModel(_backendService, _notificationService, _resourceLoader);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _viewModel.Dispose();
        }

        [TestMethod]
        public void TestSubmitCallsAddInfantIfSetByNewAndInformationIsSet()
        {
            Infant infant = null;
            Mock.Arrange(() => _backendService.AddInfant(Arg.IsAny<Infant>()))
                .DoInstead<Infant>(i => infant = i)
                .Returns(Observable.Empty<Unit>())
                .OccursOnce();

            var expectedName = "name";
            var expectedDob = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.FromHours(0));
            _viewModel.Name.Value = expectedName;
            _viewModel.DateOfBirth.Value = expectedDob;
            _viewModel.Gender.Value = new SetInfantPageViewModel.GenderItem { Gender = Infant.GenderType.Female };
            _viewModel.SubmitCommand.Execute(null);

            Assert.AreEqual(expectedName, infant.Name);
            Assert.AreEqual(expectedDob, infant.BirthDate);
            Assert.AreEqual(Infant.GenderType.Female.ToString(), infant.Gender);
            Mock.Assert(_backendService);
        }

        [TestMethod]
        public void TestSubmitDoesNotCallAddInfantWhenNotSetByNew()
        {
            Mock.Arrange(() => _backendService.AddInfant(Arg.IsAny<Infant>()))
                .Returns(Observable.Empty<Unit>())
                .OccursNever();

            _viewModel.CurrentState.Value = SetByState.Existing;
            _viewModel.Name.Value = "name";
            _viewModel.DateOfBirth.Value = DateTimeOffset.Now;
            _viewModel.Gender.Value = new SetInfantPageViewModel.GenderItem { Gender = Infant.GenderType.Female };
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [TestMethod]
        public void TestSubmitDoesNotCallAddInfantWhenNameNotSet()
        {
            Mock.Arrange(() => _backendService.AddInfant(Arg.IsAny<Infant>()))
                .Returns(Observable.Empty<Unit>())
                .OccursNever();

            _viewModel.Name.Value = null;
            _viewModel.DateOfBirth.Value = DateTimeOffset.Now;
            _viewModel.Gender.Value = new SetInfantPageViewModel.GenderItem { Gender = Infant.GenderType.Female };
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [TestMethod]
        public void TestSubmitDoesNotCallAddInfantWhenGenderNotSet()
        {
            Mock.Arrange(() => _backendService.AddInfant(Arg.IsAny<Infant>()))
                .Returns(Observable.Empty<Unit>())
                .OccursNever();

            _viewModel.Name.Value = "name";
            _viewModel.DateOfBirth.Value = DateTimeOffset.Now;
            _viewModel.Gender.Value = null;
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [TestMethod]
        public async Task TestSubmitErrorShowsErrorMessage()
        {
            bool isReady = false;
            Mock.Arrange(() => _backendService.AddInfant(Arg.IsAny<Infant>()))
                .Returns(Observable.Throw<Unit>(new Exception()));
            Mock.Arrange(() => _notificationService.ShowBlockingMessageAsync(Arg.IsAny<string>(), Arg.IsAny<string>()))
                .DoInstead<string,string>((_, __) => isReady = true)
                .Returns(Task.FromResult<object>(null))
                .OccursOnce();

            _viewModel.Name.Value = "name";
            _viewModel.DateOfBirth.Value = DateTimeOffset.Now;
            _viewModel.Gender.Value = new SetInfantPageViewModel.GenderItem { Gender = Infant.GenderType.Female };
            _viewModel.SubmitCommand.Execute(null);

            await Utilities.AssertEqualsAsync(true, () => isReady);
            Mock.Assert(_notificationService);
        }

        [TestMethod]
        public async Task TestGetExistingItemsFetchesInfants()
        {
            var infant = new Infant();
            Mock.Arrange(() => _backendService.GetInfants())
                .Returns(Observable.Return<Infant>(infant))
                .OccursOnce();

            using (var viewModel = new SetInfantPageViewModel(_backendService, _notificationService, _resourceLoader))
            {
                await Utilities.AssertEqualsAsync(1, () => viewModel.ExistingItems.Count);
                CollectionAssert.AreEquivalent(new List<Infant> { infant }, viewModel.ExistingItems);
                Mock.Assert(_backendService);
            }
        }
    }
}
