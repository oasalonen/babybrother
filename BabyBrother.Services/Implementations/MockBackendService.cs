using BabyBrother.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Services.Implementations
{
    public class MockBackendService : IBackendService
    {
        public IObservable<Unit> AddUser(User user)
        {
            return Observable.Empty<Unit>();
        }

        public IObservable<User> GetUsers()
        {
            return Observable.Empty<User>();
        }

        public IObservable<Infant> GetInfants()
        {
            return Observable.Empty<Infant>();
        }
    }
}
