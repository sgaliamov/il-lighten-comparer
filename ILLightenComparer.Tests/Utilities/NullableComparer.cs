using System.Collections.Generic;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class NullableComparer<T> : IComparer<T?> where T : struct
    {
        private readonly IComparer<T> _comparer;

        public NullableComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T? x, T? y)
        {
            if (!x.HasValue)
            {
                if (!y.HasValue)
                {
                    return 0;
                }

                return -1;
            }

            if (!y.HasValue)
            {
                return 1;
            }

            return _comparer.Compare(x.Value, y.Value);
        }
    }
}
