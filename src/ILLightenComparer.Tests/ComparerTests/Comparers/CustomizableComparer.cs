﻿using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.Comparers
{
    internal class CustomizableComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparer;

        public CustomizableComparer(Func<T, T, int> comparer) => _comparer = comparer;

        public int Compare(T x, T y) => _comparer(x, y);
    }
}