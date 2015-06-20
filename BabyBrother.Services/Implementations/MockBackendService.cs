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
        public IObservable<User> AddUser(User user)
        {
            return Observable.Empty<User>().Delay(TimeSpan.FromSeconds(3));
        }

        public IObservable<Infant> AddInfant(Infant infant)
        {
            return Observable.Empty<Infant>().Delay(TimeSpan.FromSeconds(3));
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
