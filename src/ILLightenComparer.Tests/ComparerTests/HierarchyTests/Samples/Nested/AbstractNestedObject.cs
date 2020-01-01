﻿using System;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested
{
    public abstract class AbstractNestedObject : INestedObject
    {
        public string Text { get; set; }

        public virtual int CompareTo(object obj) {
            if (ReferenceEquals(null, obj)) {
                return 1;
            }

            if (ReferenceEquals(this, obj)) {
                return 0;
            }

            return obj is AbstractNestedObject other
                       ? CompareTo(other)
                       : throw new ArgumentException($"Object must be of type {nameof(AbstractNestedObject)}.");
        }

        private int CompareTo(INestedObject other) => string.Compare(Text, other.Text, StringComparison.Ordinal);
    }
}
