using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.ViewModels.Common
{
    public enum LoadState
    {
        NotLoaded,
        Loading,
        LoadedError,
        LoadedEmpty,
        Loaded
    }
}
