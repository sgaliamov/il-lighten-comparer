﻿using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples
{
    public class NestedObject
    {
        public static IComparer<NestedObject> Comparer { get; } = new RelationalComparer();

        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        private sealed class RelationalComparer : IComparer<NestedObject>
        {
            public int Compare(NestedObject x, NestedObject y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (ReferenceEquals(null, y))
                {
                    return 1;
                }

                if (ReferenceEquals(null, x))
                {
                    return -1;
                }

                return x.Value.CompareTo(y.Value);
            }
        }
    }
}