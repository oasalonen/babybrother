using BabyBrother.Models;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Reactive;

namespace BabyBrother.Services.Implementations
{
    public class AzureBackendService : IBackendService
    {
        private readonly MobileServiceClient _service;
        private readonly IMobileServiceTable<User> _userTable;

        public AzureBackendService()
        {
            _service = new MobileServiceClient(
                "https://babymon.azure-mobile.net/",
                "NhogBGTlaNlPcmfrqFLMqrVBGBhswR26");
            _userTable = _service.GetTable<User>();
        }

        public IObservable<Unit> AddUser(User user)
        {
            return _userTable.InsertAsync(user).ToObservable();
        }

        public IObservable<User> GetUsers()
        {
            return Observable.StartAsync(() => _userTable.ToListAsync())
                .SelectMany(users => users);
        }
    }
}
