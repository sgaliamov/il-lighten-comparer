using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public abstract class AbstractNestedObject : IAbstractNestedObject
    {
        public static IComparer<AbstractNestedObject> Comparer { get; } = new RelationalComparer();
        public string Text { get; set; }

        private sealed class RelationalComparer : IComparer<AbstractNestedObject>
        {
            public int Compare(AbstractNestedObject x, AbstractNestedObject y)
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

                return string.Compare(x.Text, y.Text, StringComparison.Ordinal);
            }
        }

        public virtual int CompareTo(object obj) => Comparer.Compare(this, obj as AbstractNestedObject);
    }

    public interface IAbstractNestedObject : IComparable
    {
        string Text { get; set; }
    }
}
