﻿using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.InheritanceTests
{
    public class BaseObject
    {
        public EnumSmall? EnumField;

        public static IComparer<BaseObject> ParentObjectComparer { get; } = new ParentObjectRelationalComparer();

        public EnumBig? EnumProperty { get; set; }

        // ReSharper disable once UnusedMember.Local
        private int IgnoredPrivateProperty { get; set; }

        // ReSharper disable once UnusedMember.Global
        protected int IgnoredProtectedProperty { get; set; }

        private sealed class ParentObjectRelationalComparer : IComparer<BaseObject>
        {
            public int Compare(BaseObject x, BaseObject y)
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

                var enumFieldComparison = Nullable.Compare(x.EnumField, y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                return Nullable.Compare(x.EnumProperty, y.EnumProperty);
            }
        }
    }
}
