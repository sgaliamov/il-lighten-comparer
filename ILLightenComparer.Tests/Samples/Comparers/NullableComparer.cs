using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    // todo: remove as https://referencesource.microsoft.com/#mscorlib/system/collections/generic/comparer.cs,4bdedf7e4e4851ba
    internal sealed class NullableComparer<TValue> : IComparer<TValue?> where TValue : struct
    {
        private readonly IComparer<TValue> _valueComparer;

        public NullableComparer(IComparer<TValue> valueComparer = null)
        {
            _valueComparer = valueComparer ?? Comparer<TValue>.Default;
        }

        public int Compare(TValue? x, TValue? y)
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

            return _valueComparer.Compare(x.Value, y.Value);
        }
    }
}
