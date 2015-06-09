using BabyBrother.Services;
using BabyBrother.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace BabyBrother.UnitTest
{
    [TestClass]
    public class TestSetInfantPageViewModel
    {
        private IBackendService _backendService;
        private IResourceService _resourceLoader;
        private SetInfantPageViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            _backendService = Mock.Create<IBackendService>();
            _resourceLoader = Mock.Create<IResourceService>();
            _viewModel = new SetInfantPageViewModel(_backendService, _resourceLoader);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _viewModel.Dispose();
        }
    }
}
