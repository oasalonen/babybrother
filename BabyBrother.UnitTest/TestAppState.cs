using BabyBrother.Models;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.UnitTest
{
    [TestClass]
    public class TestAppState
    {
        private AppState _appState;

        [TestInitialize]
        public void Initialize()
        {
            _appState = new AppState();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _appState.Dispose();
        }

        [TestMethod]
        public async Task TestSetCurrentUserEmitsUser()
        {
            var user = new User() { Id = "1" };
            await _appState.CurrentUserStream.AssertNextValueIs(user, () => _appState.SetCurrentUser(user));
        }

        [TestMethod]
        public async Task TestSetInfantEmitsInfant()
        {
            var infant = new Infant() { Id = "1" };
            await _appState.CurrentInfantStream.AssertNextValueIs(infant, () => _appState.SetCurrentInfant(infant));
        }

        [TestMethod]
        public async Task TestSetNullUserDoesNotEmitUser()
        {
            await _appState.CurrentUserStream.AssertEmitsNone(() => _appState.SetCurrentUser(null));
        }

        [TestMethod]
        public async Task TestSetNullInfantDoesNotEmitInfant()
        {
            await _appState.CurrentInfantStream.AssertEmitsNone(() => _appState.SetCurrentInfant(null));
        }
    }
}
