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

namespace BabyBrother.UnitTest
{
    [TestClass]
    public class TestSetUserPageViewModel
    {
        private SetUserPageViewModel _viewModel;
        private IBackendService _backendService;

        [TestInitialize]
        public void Initialize()
        {
            // Needed if not manually passing IScheduler to ReactiveCommands?
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            _backendService = Mock.Create<IBackendService>();
            _viewModel = new SetUserPageViewModel(_backendService);
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
            Assert.AreEqual(SetUserPageViewModel.State.SetByNew, _viewModel.CurrentState.Value);
        }

        [TestMethod]
        public void TestIsExistingUsersAvailableIsFalseInitially()
        {
            Assert.AreEqual(false, _viewModel.IsExistingUsersAvailable.Value);
        }

        [TestMethod]
        public void TestSetByExistingCommandSetsStateToExisting()
        {
            _viewModel.SetByExistingCommand.Execute(null);
            Assert.AreEqual(SetUserPageViewModel.State.SetByExisting, _viewModel.CurrentState.Value);
        }

        [TestMethod]
        public void TestSetByNewCommandSetsStateToNew()
        {
            _viewModel.CurrentState.Value = SetUserPageViewModel.State.SetByExisting;
            _viewModel.SetByNewCommand.Execute(null);
            Assert.AreEqual(SetUserPageViewModel.State.SetByNew, _viewModel.CurrentState.Value);
        }

        [TestMethod]
        public void TestSubmitCommandAddsUserWhenSetByNewAndNameIsSet()
        {
            var expectedName = "name";
            User calledUser = null;

            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .DoInstead<User>(u => calledUser = u)
                .Returns(Observable.Empty<Unit>())
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
                .Returns(Observable.Empty<Unit>())
                .OccursNever();

            _viewModel.NewUsername.Value = name;
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [TestMethod]
        public void TestSubmitCommandDoesNothingWhenSetByExisting()
        {
            Mock.Arrange(() => _backendService.AddUser(Arg.IsAny<User>()))
                .Returns(Observable.Empty<Unit>())
                .OccursNever();

            _viewModel.NewUsername.Value = "name";
            _viewModel.SetByExistingCommand.Execute(null);
            _viewModel.SubmitCommand.Execute(null);

            Mock.Assert(_backendService);
        }

        [DataTestMethod]
        [DataRow("", SetUserPageViewModel.State.SetByNew, false, false, DisplayName = "Set by new with no name cannot exec")]
        [DataRow("a", SetUserPageViewModel.State.SetByNew, false, true, DisplayName = "Set by new with name can exec")]
        [DataRow("", SetUserPageViewModel.State.SetByExisting, false, false, DisplayName = "Set by existing with no selection cannot exec")]
        [DataRow("", SetUserPageViewModel.State.SetByExisting, true, true, DisplayName = "Set by existing with selection can exec")]
        public void TestSubmitCommandCanExecuteIsSetCorrectly(string name, SetUserPageViewModel.State state, bool isUserSelected, bool expectedCanExecute)
        {
            _viewModel.NewUsername.Value = name;
            _viewModel.CurrentState.Value = state;
            _viewModel.SelectExistingUser(isUserSelected ? new User { Name = name } : null);
            Assert.AreEqual(expectedCanExecute, _viewModel.SubmitCommand.CanExecute(null));
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

            var viewModel = new SetUserPageViewModel(backendService);
            CollectionAssert.AreEquivalent(users, viewModel.ExistingUsers);
            Mock.Assert(backendService);
        }

        [TestMethod]
        public void TestIsExistingUsersAvailableIsFalseWhenBackendReturnsNone()
        {
            Assert.IsFalse(_viewModel.IsExistingUsersAvailable.Value);
        }

        [TestMethod]
        public void TestIsExistingUsersAvailableIsTrueWhenBackendReturnsSome()
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

            var viewModel = new SetUserPageViewModel(backendService);
            Assert.IsTrue(viewModel.IsExistingUsersAvailable.Value);
            Mock.Assert(backendService);
        }
    }
}
