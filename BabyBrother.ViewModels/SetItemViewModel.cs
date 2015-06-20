using BabyBrother.ViewModels.Common;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
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
        public enum RequestedAction
        {
            Complete
        }

        private readonly ISubject<T> _itemSelectionStream;
        private readonly ISubject<IObservable<Notification<T>>> _submitStream;
        protected readonly IObservable<Notification<T>> _submitStatusStream;
        protected readonly IObservable<T> _setItemStream;

        public ReactiveProperty<SetByState> CurrentState { get; private set; }

        public ReactiveCollection<T> ExistingItems { get; private set; }

        public ReactiveProperty<LoadState> ExistingItemsLoadState { get; private set; }

        public ReactiveProperty<bool> IsSubmitting { get; private set; }

        public IObservable<RequestedAction> ActionStream { get; private set; }

        public ICommand SetByNewCommand { get; private set; }

        public ICommand SetByExistingCommand { get; private set; }

        public ICommand SubmitCommand { get; private set; }

        public SetItemViewModel()
        {
            _submitStream = new Subject<IObservable<Notification<T>>>();
            _submitStatusStream = _submitStream.Switch();
            _itemSelectionStream = new BehaviorSubject<T>(null);

            _setItemStream = _submitStatusStream
                .Where(notification => notification.Kind == NotificationKind.OnNext && notification.HasValue && notification.Value != null)
                .Select(notification => notification.Value);

            ActionStream = _setItemStream.Select(_ => RequestedAction.Complete);

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

        protected void InitializeSubmit()
        {
            // Submit setup
            IsSubmitting = _submitStream.Select(_ => true)
                .Merge(_submitStatusStream.Select(notification => notification.Kind == NotificationKind.OnNext))
                .Catch(Observable.Empty<bool>())
                .ToReactiveProperty(false);
            AddSubscription(IsSubmitting);

            var isExistingUserSelectedStream = _itemSelectionStream
                .Select(item => item != null)
                .CombineLatest(CurrentState, (isItemSelected, state) => isItemSelected && state == SetByState.Existing);
            var canSubmitStream = isExistingUserSelectedStream
                .CombineLatest(IsReadyToSubmit(), (isItemSelected, isReadyToSubmit) => isItemSelected || isReadyToSubmit)
                .CombineLatest(IsSubmitting, (isReadyToSubmit, isSubmitting) => isReadyToSubmit && !isSubmitting);
            var submitCommand = new ReactiveCommand(canSubmitStream);
            AddSubscription(submitCommand.Subscribe(_ => 
            {
                if (CurrentState.Value == SetByState.New)
                {
                    _submitStream.OnNext(OnSubmit()
                        .Materialize()
                        .Catch(Observable.Empty<Notification<T>>()));
                }
                else
                {
                    _submitStream.OnNext(_itemSelectionStream
                        .Materialize());
                }
            }));
            SubmitCommand = submitCommand;

            AddSubscription(_submitStatusStream
                .Where(notification => notification.Kind == NotificationKind.OnError)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => OnSubmitError()));
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

        public void SelectExistingItem(T item)
        {
            _itemSelectionStream.OnNext(item);
        }

        protected abstract IObservable<T> OnSubmit();

        protected abstract void OnSubmitError();

        protected abstract IObservable<bool> IsReadyToSubmit();
    }
}
