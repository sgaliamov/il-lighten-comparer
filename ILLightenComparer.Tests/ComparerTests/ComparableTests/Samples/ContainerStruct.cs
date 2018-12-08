using System;
using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public struct ContainerStruct
    {
        public ComparableObject ComparableField;
        public ComparableStruct ComparableStructField;
        public ComparableStruct? ComparableStructNullableField;

        public static IComparer<ContainerStruct> Comparer { get; } = new RelationalComparer();

        public ComparableChildObject ComparableProperty { get; set; }
        public ComparableStruct? ComparableStructNullableProperty { get; set; }
        public ComparableStruct ComparableStructProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ContainerStruct>
        {
            public int Compare(ContainerStruct x, ContainerStruct y)
            {
                var compare = x.ComparableField.CompareTo(y.ComparableField);
                if (compare != 0) { return compare; }

                compare = x.ComparableStructField.CompareTo(y.ComparableStructField);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableField,
                    y.ComparableStructNullableField);
                if (compare != 0) { return compare; }

                compare = x.ComparableProperty.CompareTo(y.ComparableProperty);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableProperty,
                    y.ComparableStructNullableProperty);
                if (compare != 0) { return compare; }

                compare = Comparer<ComparableStruct>.Default.Compare(
                    x.ComparableStructProperty,
                    y.ComparableStructProperty);
                if (compare != 0) { return compare; }

                return 0;
            }
        }
    }
}
