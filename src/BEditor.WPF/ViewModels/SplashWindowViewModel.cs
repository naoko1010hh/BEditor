﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reactive.Bindings;

namespace BEditor.ViewModels
{
    public class SplashWindowViewModel
    {
        public ReactiveProperty<string> Status { get; } = new();
    }
}
