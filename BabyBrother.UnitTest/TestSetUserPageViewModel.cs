using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using BabyBrother.ViewModels;
using BabyBrother.Services;
using Telerik.JustMock;
using System.Threading;
using BabyBrother.Models;
using System.Reactive.Linq;
using System.Reactive;
using BabyBrother.ViewModels.Common;
using System.Threading.Tasks;

namespace BabyBrother.UnitTest
{
    [TestClass]
    public class TestSetUserPageViewModel
    {
        private SetUserPageViewModel _viewModel;
        private AppState _appState;
        private IBackendService _backendService;
        private INotificationService _notificationService;
        private IResourceService _resourceService;

        [TestInitialize]
        public void Initialize()
        {
            // Needed if not manually passing IScheduler to ReactiveCommands?
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            _appState = new AppState();
            _backendService = Mock.Create<IBackendService>();
            _notificationService = Mock.Create<INotificationService>();
            _resourceService = Mock.Create<IResourceService>();
            _viewModel = new SetUserPageViewModel(_appState, _backendService, _notificationService, _resourceService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _viewModel.Dispose();
        }

        [TestMethod]
        public void TestCannotSubmitInitially()
        {
            Assert.AreEqual(false, _viewModel.SubmitCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestInitialStateIsSetByNew()
        {
            Assert.AreEqual(SetByState.New, _viewModel.CurrentState.Value);
        }


        [TestMethod]
        public async Task TestSetByExistingCommandSetsStateToExisting()
        {
            var expected = new List<SetByState> { SetByState.New, SetByState.Existing };
            await _viewModel.CurrentState.AssertNextValueIs(expected, () => _viewModel.SetByExistingCommand.Execute(null));
        }

        [TestMethod]
        public async Task TestSetByNewCommandSetsStateToNew()
        {
            _viewModel.CurrentState.Value = SetByState.Existing;

            var expected = new List<SetByState> { SetByState.Existing, SetByState.New };
            await _viewModel.CurrentState.AssertNextValueIs(expected, () => _viewModel.SetByNewCommand.Execute(null));
        }

        [TestMethod]
        public void TestSubmitCommandAddsUserWhenSetByNewAndNameIsSet()
        {
            var expectedName = "name";
            User calledUser = null;

            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .DoInstead<User>(u => calledUser = u)
                .Returns(Observable.Empty<User>())
                .OccursOnce();

            _viewModel.NewUsername.Value = expectedName;
            _viewModel.SubmitCommand.Execute(null);

            Assert.AreEqual(expectedName, calledUser.Name);
            Mock.Assert(_backendService);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null username")]
        [DataRow("", DisplayName = "Empty username")]
        [DataRow(" ", DisplayName = "Whitespace username")]
        public void TestSubmitCommandDoesNothingWhenSetByNewAndInvalidName(string name)
        {
            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .Returns(Observable.Empty<User>())
                .OccursNever();

            _viewModel.NewUsername.Value = name;
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [TestMethod]
        public void TestSubmitCommandDoesNothingWhenSetByExisting()
        {
            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .Returns(Observable.Empty<User>())
                .OccursNever();

            _viewModel.NewUsername.Value = "name";
            _viewModel.SetByExistingCommand.Execute(null);
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [DataTestMethod]
        [DataRow("", SetByState.New, false, false, DisplayName = "Set by new with no name cannot exec")]
        [DataRow("a", SetByState.New, false, true, DisplayName = "Set by new with name can exec")]
        [DataRow("", SetByState.Existing, false, false, DisplayName = "Set by existing with no selection cannot exec")]
        [DataRow("", SetByState.Existing, true, true, DisplayName = "Set by existing with selection can exec")]
        public void TestSubmitCommandCanExecuteIsSetCorrectly(string name, SetByState state, bool isUserSelected, bool expectedCanExecute)
        {
            _viewModel.NewUsername.Value = name;
            _viewModel.CurrentState.Value = state;
            _viewModel.SelectExistingItem(isUserSelected ? new User { Name = name } : null);
            Assert.AreEqual(expectedCanExecute, _viewModel.SubmitCommand.CanExecute(null));
        }

        [TestMethod]
        public async Task TestSubmitCommandSetsIsSubmittingToTrueWhenAddingUser()
        {
            _viewModel.NewUsername.Value = "name";
            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .Returns(() => Observable.Never<User>());

            var expectedValues = new List<bool> { false, true };
            await _viewModel.IsSubmitting.AssertNextValueIs(expectedValues, () => _viewModel.SubmitCommand.Execute(null));
        }

        [TestMethod]
        public void TestGetsExistingUsersWhenConstructed()
        {
            var backendService = Mock.Create<IBackendService>();
            var users = new List<User>
            {
                new User { Name = "a" },
                new User { Name = "b" }
            };
            Mock.Arrange(() => backendService.GetUsers())
                .Returns(() => users.ToObservable())
                .OccursOnce();

            var viewModel = new SetUserPageViewModel(_appState, backendService, _notificationService, _resourceService);
            CollectionAssert.AreEquivalent(users, viewModel.ExistingItems);
            Mock.Assert(backendService);
        }

        [TestMethod]
        public async Task TestExistingItemsExistWhenBackendReturnsSome()
        {
            var backendService = Mock.Create<IBackendService>();
            var users = new List<User>
            {
                new User { Name = "a" },
                new User { Name = "b" }
            };
            Mock.Arrange(() => backendService.GetUsers())
                .Returns(() => users.ToObservable())
                .MustBeCalled();

            var viewModel = new SetUserPageViewModel(_appState, backendService, _notificationService, _resourceService);
            await Utilities.AssertEqualsAsync(2, () => viewModel.ExistingItems.Count);
            Mock.Assert(backendService);
        }

        [TestMethod]
        public void TestGetExistingUsersExceptionResultsInEmptyUserList()
        {
            var backendService = Mock.Create<IBackendService>();
            Mock.Arrange(() => backendService.GetUsers())
                .Returns(() => Observable.Throw<User>(new Exception()));

            var viewModel = new SetUserPageViewModel(_appState, backendService, _notificationService, _resourceService);
            Assert.AreEqual(0, viewModel.ExistingItems.Count);
        }

        [TestMethod]
        public void TestExistingUsersLoadStateIsLoadingInitially()
        {
            Mock.Arrange(() => _backendService.GetUsers())
                .Returns(() => Observable.Never<User>());

            using (var viewModel = CreateViewModel())
            {
                Assert.AreEqual(LoadState.Loading, viewModel.ExistingItemsLoadState.Value);
            }
        }

        [TestMethod]
        public async Task TestExistingUsersLoadStateIsEmptyIfNoUsersReturned()
        {
            Mock.Arrange(() => _backendService.GetUsers())
                .Returns(() => Observable.Empty<User>());

            using (var viewModel = CreateViewModel())
            {
                await viewModel.ExistingItemsLoadState.AssertNextValueIs(LoadState.LoadedEmpty);
            }
        }

        [TestMethod]
        public async Task TestExistingUsersLoadStateIsLoadedIfUsersReturned()
        {
            Mock.Arrange(() => _backendService.GetUsers())
                .Returns(() => Observable.Return(new User()));

            using (var viewModel = CreateViewModel())
            {
                await viewModel.ExistingItemsLoadState.AssertNextValueIs(LoadState.Loaded);
            }
        }

        [TestMethod]
        public async Task TestExistingUsersLoadStateIsErrorIfExceptionIsThrown()
        {
            Mock.Arrange(() => _backendService.GetUsers())
                .Returns(() => Observable.Throw<User>(new Exception()));

            using (var viewModel = CreateViewModel())
            {
                await viewModel.ExistingItemsLoadState.AssertNextValueIs(LoadState.LoadedError);
            }
        }

        [TestMethod]
        public async Task TestSuccessfulSubmitRequestsCompleteAction()
        {
            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .Returns(() => Observable.Return(new User()));

            _viewModel.NewUsername.Value = "name";
            await _viewModel.ActionStream.AssertNextValueIs(SetUserPageViewModel.RequestedAction.Complete,
                () => _viewModel.SubmitCommand.Execute(null));
        }

        // TODO: this fails because Mock.Assert needs to be spin-delayed
        //[TestMethod]
        //public void TestFailedSubmitShowsError()
        //{
        //    Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
        //        .Returns(() => Observable.Throw<Unit>(new Exception()));
        //    Mock.Arrange(() => _notificationService.ShowBlockingMessageAsync(Arg.IsAny<string>(), Arg.IsAny<string>()))
        //        .OccursOnce();

        //    _viewModel.NewUsername.Value = "name";
        //    _viewModel.SubmitCommand.Execute(null);
        //    Mock.Assert(_notificationService);
        //}

        [TestMethod]
        public async Task TestSubmittingWithExistingUserRequestsCompleteAction()
        {
            _viewModel.SetByExistingCommand.Execute(null);
            _viewModel.SelectExistingItem(new User());

            await _viewModel.ActionStream.AssertNextValueIs(SetUserPageViewModel.RequestedAction.Complete,
                () => _viewModel.SubmitCommand.Execute(null));
        }

        [TestMethod]
        public async Task TestAddingNewUserSetsAppStateCurrentUser()
        {
            var user = new User { Id = "id", Name = "name" };

            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .Returns(() => Observable.Return(user));

            _viewModel.NewUsername.Value = user.Name;
            _viewModel.SubmitCommand.Execute(null);
            await _appState.CurrentUserStream.AssertNextValueIs(user);
        }

        [TestMethod]
        public async Task TestSetByExistingSetsAppStateCurrentUser()
        {
            var user = new User { Id = "id", Name = "name" };
            _viewModel.SetByExistingCommand.Execute(null);
            _viewModel.SelectExistingItem(user);
            _viewModel.SubmitCommand.Execute(null);
            await _appState.CurrentUserStream.AssertNextValueIs(user);
        }

        private SetUserPageViewModel CreateViewModel()
        {
            return new SetUserPageViewModel(_appState, _backendService, _notificationService, _resourceService);
        }
    }
}
