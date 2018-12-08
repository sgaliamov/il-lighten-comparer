using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public class ContainerObject
    {
        public ChildComparableObject ComparableField;
        public ComparableStruct ComparableStructField;

        public ComparableStruct? ComparableStructNullableField;

        public static IComparer<ContainerObject> Comparer { get; } = new RelationalComparer();

        public ComparableObject ComparableProperty { get; set; }
        public ComparableStruct? ComparableStructNullableProperty { get; set; }
        public ComparableStruct ComparableStructProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ContainerObject>
        {
            public int Compare(ContainerObject x, ContainerObject y)
            {
                if (ReferenceEquals(x, y)) { return 0; }

                if (ReferenceEquals(null, y)) { return 1; }

                if (ReferenceEquals(null, x)) { return -1; }

                var compare = Comparer<ComparableObject>.Default.Compare(
                    x.ComparableField,
                    y.ComparableField);
                if (compare != 0) { return compare; }

                compare = x.ComparableStructField.CompareTo(y.ComparableStructField);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableField,
                    y.ComparableStructNullableField);
                if (compare != 0) { return compare; }


                compare = Comparer<ComparableObject>.Default.Compare(
                    x.ComparableProperty,
                    y.ComparableProperty);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableProperty,
                    y.ComparableStructNullableProperty);
                if (compare != 0) { return compare; }

                compare = Comparer<ComparableStruct>.Default.Compare(
                    x.ComparableStructProperty,
                    y.ComparableStructProperty);
                if (compare != 0) { return compare; }

                return compare;
            }
        }
    }
}
