using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.InheritanceTests
{
    public sealed class ChildObject : BaseObject
    {
        public int Field;

        public static IComparer<ChildObject> ChildObjectComparer { get; } = new ChildObjectRelationalComparer();

        public string Property { get; set; }

        private sealed class ChildObjectRelationalComparer : IComparer<ChildObject>
        {
            public int Compare(ChildObject x, ChildObject y)
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

                var fieldComparison = x.Field.CompareTo(y.Field);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                var compare = string.Compare(x.Property, y.Property, StringComparison.Ordinal);
                if (compare != 0)
                {
                    return compare;
                }

                return ParentObjectComparer.Compare(x, y);
            }
        }
    }
}
