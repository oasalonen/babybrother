using BabyBrother.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BabyBrother.ViewModels
{
    public class FeedPageViewModel : ViewModel
    {
        public ReactiveProperty<Feeding.Source> Source { get; private set; }

        public ReactiveProperty<DateTimeOffset> StartTime { get; private set; }

        public ReactiveProperty<DateTimeOffset> StopTime { get; private set; }

        public ICommand ToggleLeftBreast { get; private set; }

        public ICommand ToggleRightBreast { get; private set; }

        public ICommand ToggleBottle { get; private set; }

        public ICommand Start { get; private set; }

        public ICommand Stop { get; private set; }
    }
}
