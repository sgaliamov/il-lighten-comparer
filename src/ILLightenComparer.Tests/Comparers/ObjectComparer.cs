using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.Comparers
{
    internal sealed class ObjectComparer : IComparer<object>, IComparer
    {
        public int Compare(object x, object y)
        {
            if (x is null) {
                return y is null ? 0 : -1;
            }

            return y is null ? 1 : 0;
        }
    }
}
