using BabyBrother.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;

namespace BabyBrother.Services
{
    public interface IBackendService
    {
        IObservable<User> AddUser(User user);

        IObservable<Infant> AddInfant(Infant infant);

        IObservable<User> GetUsers();

        IObservable<Infant> GetInfants();
    }
}
