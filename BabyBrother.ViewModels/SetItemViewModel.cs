using BabyBrother.ViewModels.Common;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BabyBrother.ViewModels
{
    public enum SetByState
    {
        New,
        Existing
    }

    public abstract class SetItemViewModel<T> : ViewModel 
        where T : class
    {
        public ReactiveProperty<SetByState> CurrentState { get; private set; }

        public ReactiveCollection<T> ExistingItems { get; private set; }

        public ReactiveProperty<LoadState> ExistingItemsLoadState { get; private set; }

        public ICommand SetByNewCommand { get; private set; }

        public ICommand SetByExistingCommand { get; private set; }

        public SetItemViewModel()
        {
            // State switching setup
            var setUserByNew = new ReactiveCommand();
            var setUserByExisting = new ReactiveCommand();

            SetByNewCommand = setUserByNew;
            SetByExistingCommand = setUserByExisting;

            var setByNewStream = setUserByNew.Select(_ => SetByState.New);
            var setByExistingStream = setUserByExisting.Select(_ => SetByState.Existing);

            CurrentState = setByNewStream
                .Merge(setByExistingStream)
                .ToReactiveProperty(SetByState.New);
            AddSubscription(CurrentState);
        }

        protected void InitializeExistingItems(IObservable<T> itemStream)
        {
            ExistingItems = itemStream
                .Catch(Observable.Empty<T>())
                .ToReactiveCollection();
            AddSubscription(ExistingItems);

            ExistingItemsLoadState = itemStream.Materialize()
                .Select(notification => notification.Kind == NotificationKind.OnError ? LoadState.LoadedError : LoadState.Loaded)
                .CombineLatest(itemStream.Any(), (loadState, isAny) =>
                {
                    return loadState == LoadState.LoadedError ?
                        LoadState.LoadedError :
                        (isAny ? LoadState.Loaded : LoadState.LoadedEmpty);
                })
                .StartWith(LoadState.Loading)
                .Catch(Observable.Return(LoadState.LoadedError))
                .ToReactiveProperty();
            AddSubscription(ExistingItemsLoadState);
        }

        public abstract void SelectExistingItem(T item);
    }
}
