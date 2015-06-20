using BabyBrother.Models;
using BabyBrother.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BabyBrother.UnitTest
{
    [TestClass]
    public class TestFeedPageViewModel
    {
        private FeedPageViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            _viewModel = new FeedPageViewModel();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _viewModel.Dispose();
        }

        [TestMethod]
        public async Task TestInitialSourceIsNone()
        {
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.None);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.None));
        }

        [TestMethod]
        public async Task TestToggleLeftBreastSetsSourceToLeft()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Left);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Left));
        }

        [TestMethod]
        public async Task TestToggleLeftLeftBreastSetsSourceToNone()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleLeftBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.None);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.None));
        }

        [TestMethod]
        public async Task TestToggleRightBreastSetsSourceToRight()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Right);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Right));
        }

        [TestMethod]
        public async Task TestToggleRightRightBreastSetsSourceToNone()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.None);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.None));
        }

        [TestMethod]
        public async Task TestToggleRightAndLeftBreastSetsSourceToLeftRight()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.LeftRight);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.LeftRight));
        }

        [TestMethod]
        public async Task TestToggleRightAndLeftAndRightBreastSetsSourceToLeft()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Left);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Left));
        }

        [TestMethod]
        public async Task TestToggleBottleLeftSetsSourceToLeft()
        {
            _viewModel.ToggleBottle.Execute(null);
            _viewModel.ToggleLeftBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Left);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Left));
        }

        [TestMethod]
        public async Task TestToggleBottleRightSetsSourceToRight()
        {
            _viewModel.ToggleBottle.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Right);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Right));
        }

        [TestMethod]
        public async Task TestToggleBottleBottleSetsSourceToNone()
        {
            _viewModel.ToggleBottle.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.None);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.None));
        }

        [TestMethod]
        public async Task TestToggleLeftBottleSetsSourceToBottle()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Bottle);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Bottle));
        }

        [TestMethod]
        public async Task TestToggleRightBottleSetsSourceToBottle()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Bottle);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Bottle));
        }

        [TestMethod]
        public async Task TestToggleLeftRightBottleSetsSourceToBottle()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await _viewModel.Source.AssertNextValueIs(Feeding.Source.Bottle);
            Assert.IsTrue(IsSourceSelectionsValid(_viewModel, Feeding.Source.Bottle));
        }

        [TestMethod]
        public async Task TestStartSetsStartTime()
        {
            _viewModel.Start.Execute(null);
            await Utilities.AssertIsApproximateTimeAsync(DateTimeOffset.Now, () => _viewModel.StartTime.Value);
        }

        [TestMethod]
        public void TestCannotStartIfStartTimeSet()
        {
            _viewModel.StartTime.Value = DateTimeOffset.Now;
            Assert.IsFalse(_viewModel.Start.CanExecute(null));
        }

        [TestMethod]
        public async Task TestCannotStopIfStartTimeNotSet()
        {
            await Utilities.AssertEqualsAsync(false, () => _viewModel.Stop.CanExecute(null));
        }

        [TestMethod]
        public async Task TestCanStopIfStartTimeSet()
        {
            _viewModel.StartTime.Value = DateTimeOffset.Now;
            await Utilities.AssertEqualsAsync(true, () => _viewModel.Stop.CanExecute(null));
        }

        [TestMethod]
        public async Task TestStopSetsStopTime()
        {
            _viewModel.StartTime.Value = DateTimeOffset.Now - TimeSpan.FromHours(1);
            _viewModel.Stop.Execute(null);
            await Utilities.AssertIsApproximateTimeAsync(DateTimeOffset.Now, () => _viewModel.StopTime.Value);
        }

        [TestMethod]
        public async Task TestIsInProgressIsTrueAfterStart()
        {
            await _viewModel.IsRunning.AssertNextValueIs(new List<bool>() { false, true }, () => _viewModel.Start.Execute(null));
        }

        [TestMethod]
        public async Task TestIsInProgressIsFalseAfterStop()
        {
            await _viewModel.IsRunning.AssertNextValueIs(new List<bool>() { false, true, false }, () =>
            {
                _viewModel.Start.Execute(null);
                _viewModel.Stop.Execute(null);
            });
        }

        [TestMethod]
        public async Task TestDurationIsSetUntilNowIfStartTimeIsSet()
        {
            var expectedDuration = TimeSpan.FromHours(1);
            _viewModel.StartTime.Value = DateTimeOffset.Now - expectedDuration;
            await Utilities.AssertIsApproximateTimeAsync(expectedDuration, () => _viewModel.Duration.Value);
        }

        [TestMethod]
        public async Task TestDurationIsSetUntilStopTimeIfStartAndStopTimeIsSet()
        {
            var expectedDuration = TimeSpan.FromHours(1);
            _viewModel.StartTime.Value = DateTimeOffset.Now;
            _viewModel.StopTime.Value = DateTimeOffset.Now + expectedDuration;
            await Utilities.AssertIsApproximateTimeAsync(expectedDuration, () => _viewModel.Duration.Value);
        }

        public bool IsSourceSelectionsValid(FeedPageViewModel viewModel, Feeding.Source source)
        {
            switch (source)
            {
                case Feeding.Source.None:
                    return !viewModel.IsBottleSelected.Value && !viewModel.IsLeftBreastSelected.Value && !viewModel.IsRightBreastSelected.Value;
                case Feeding.Source.Bottle:
                    return viewModel.IsBottleSelected.Value && !viewModel.IsLeftBreastSelected.Value && !viewModel.IsRightBreastSelected.Value;
                case Feeding.Source.LeftRight:
                    return !viewModel.IsBottleSelected.Value && viewModel.IsLeftBreastSelected.Value && viewModel.IsRightBreastSelected.Value;
                case Feeding.Source.Left:
                    return !viewModel.IsBottleSelected.Value && viewModel.IsLeftBreastSelected.Value && !viewModel.IsRightBreastSelected.Value;
                case Feeding.Source.Right:
                    return !viewModel.IsBottleSelected.Value && !viewModel.IsLeftBreastSelected.Value && viewModel.IsRightBreastSelected.Value;
                default:
                    return false;
            }
        }
    }
}
