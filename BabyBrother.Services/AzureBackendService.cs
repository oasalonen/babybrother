using BabyBrother.Models;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Services
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

        public async Task AddUser(User user)
        {
            //return Observable.StartAsync(() => _userTable.InsertAsync(user));
            await _userTable.InsertAsync(user);
        }

        public IObservable<IEnumerable<User>> GetUsers()
        {
            return Observable.StartAsync(() => _userTable.ToListAsync());
        }
    }
}
