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
                .MustBeCalled();

            var viewModel = new SetUserPageViewModel(backendService);
            CollectionAssert.AreEquivalent(users, viewModel.ExistingUsers);
        }
    }
}
