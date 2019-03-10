using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal class CustomisableComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparer;

        public CustomisableComparer(Func<T, T, int> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }
}
