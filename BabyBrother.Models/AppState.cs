using BabyBrother.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Models
{
    /// <summary>
    /// Should be kept thread-safe, used as a singleton.
    /// </summary>
    public class AppState : SubscriptionDisposable
    {
        public IObservable<User> CurrentUserStream
        {
            get { return _currentUserStream.Where(user => user != null); }
        }
        private readonly ReplaySubject<User> _currentUserStream;

        public IObservable<Infant> CurrentInfantStream
        {
            get { return _currentInfantStream.Where(infant => infant != null); }
        }
        private readonly ReplaySubject<Infant> _currentInfantStream;

        public AppState()
        {
            _currentUserStream = new ReplaySubject<User>(1);
            AddSubscription(_currentUserStream);

            _currentInfantStream = new ReplaySubject<Infant>(1);
            AddSubscription(_currentInfantStream);
        }

        public void SetCurrentUser(User user)
        {
            _currentUserStream.OnNext(user);
        }

        public void SetCurrentInfant(Infant infant)
        {
            _currentInfantStream.OnNext(infant);
        }
    }
}
