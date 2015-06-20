using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Base
{
    public class SubscriptionDisposable : IDisposable
    {
        private readonly CompositeDisposable _subscriptions;

        public SubscriptionDisposable()
        {
            _subscriptions = new CompositeDisposable();
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        protected void AddSubscription(IDisposable disposable)
        {
            _subscriptions.Add(disposable);
        }
    }
}
