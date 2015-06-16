using BabyBrother.Models;
using BabyBrother.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _viewModel = new FeedPageViewModel();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _viewModel.Dispose();
        }

        [TestMethod]
        public void TestInitialSourceIsNone()
        {
            Assert.AreEqual(Feeding.Source.None, _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleLeftBreastSetsSourceToLeft()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Left, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleLeftLeftBreastSetsSourceToNone()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleLeftBreast.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.None, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleRightBreastSetsSourceToRight()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Right, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleRightRightBreastSetsSourceToNone()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.None, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleRightAndLeftBreastSetsSourceToLeftRight()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.LeftRight, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleRightAndLeftAndRightBreastSetsSourceToLeft()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Left, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleBottleSetsSourceToBottle()
        {
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Bottle, () => _viewModel.Source.Value);

            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.None, () => _viewModel.Source.Value);

            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Bottle, () => _viewModel.Source.Value);

            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Bottle, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleBottleBottleSetsSourceToNone()
        {
            _viewModel.ToggleBottle.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.None, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleLeftBottleSetsSourceToBottle()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Bottle, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleRightBottleSetsSourceToBottle()
        {
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Bottle, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestToggleLeftRightBottleSetsSourceToBottle()
        {
            _viewModel.ToggleLeftBreast.Execute(null);
            _viewModel.ToggleRightBreast.Execute(null);
            _viewModel.ToggleBottle.Execute(null);
            await Utilities.AssertEqualsAsync(Feeding.Source.Bottle, () => _viewModel.Source.Value);
        }

        [TestMethod]
        public async Task TestStartSetsStartTime()
        {
            _viewModel.Start.Execute(null);
            await Utilities.AssertIsApproximateTimeAsync(DateTimeOffset.Now, () => _viewModel.StartTime.Value);
        }

        [TestMethod]
        public void TestCannotStopIfStartTimeNotSet()
        {
            Assert.IsFalse(_viewModel.Stop.CanExecute(null));
        }

        [TestMethod]
        public void TestCanStopIfStartTimeSet()
        {
            _viewModel.StartTime.Value = DateTimeOffset.Now;
            Assert.IsTrue(_viewModel.Stop.CanExecute(null));
        }

        [TestMethod]
        public async Task TestStopSetsStopTime()
        {
            _viewModel.StartTime.Value = DateTimeOffset.Now - TimeSpan.FromHours(1);
            _viewModel.Stop.Execute(null);
            await Utilities.AssertIsApproximateTimeAsync(DateTimeOffset.Now, () => _viewModel.StopTime.Value);
        }
    }
}
