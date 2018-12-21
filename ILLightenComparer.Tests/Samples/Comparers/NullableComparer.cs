using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal sealed class NullableComparer<TValue> : IComparer<TValue?>, IComparer
        where TValue : struct
    {
        private readonly IComparer<TValue> _valueComparer;

        public NullableComparer(IComparer<TValue> valueComparer = null)
        {
            _valueComparer = valueComparer ?? Comparer<TValue>.Default;
        }

        public int Compare(object x, object y)
        {
            return Compare((TValue)x, (TValue)y);
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
