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
            return Observable.Empty<Unit>().Delay(TimeSpan.FromSeconds(3));
        }

        public IObservable<Unit> AddInfant(Infant infant)
        {
            return Observable.Empty<Unit>().Delay(TimeSpan.FromSeconds(3));
        }

        public IObservable<User> GetUsers()
        {
            return Observable.Return(new User { Name = "joe" }).Delay(TimeSpan.FromSeconds(3));
        }

        public IObservable<Infant> GetInfants()
        {
            return Observable.Return(new Infant { Name = "babyjim" }).Delay(TimeSpan.FromSeconds(3));
        }
    }
}
