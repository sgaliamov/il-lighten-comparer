using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public struct ContainerStruct
    {
        public SampleComparableObject ComparableField;
        public SampleComparableStruct<EnumBig> ComparableStructField;
        public SampleComparableStruct<EnumSmall>? ComparableStructNullableField;

        public static IComparer<ContainerStruct> Comparer { get; } = new RelationalComparer();

        public SampleComparableChildObject ComparableProperty { get; set; }
        public SampleComparableStruct<decimal>? ComparableStructNullableProperty { get; set; }
        public SampleComparableStruct<string> ComparableStructProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ContainerStruct>
        {
            public int Compare(ContainerStruct x, ContainerStruct y)
            {
                var compare = Comparer<SampleComparableObject>.Default.Compare(
                    x.ComparableField,
                    y.ComparableField);
                if (compare != 0) { return compare; }

                compare = x.ComparableStructField.CompareTo(y.ComparableStructField);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableField,
                    y.ComparableStructNullableField);
                if (compare != 0) { return compare; }

                compare = Comparer<SampleComparableChildObject>.Default.Compare(
                    x.ComparableProperty,
                    y.ComparableProperty);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableProperty,
                    y.ComparableStructNullableProperty);
                if (compare != 0) { return compare; }

                compare = x.ComparableStructProperty.CompareTo(y.ComparableStructProperty);
                if (compare != 0) { return compare; }

                return 0;
            }
        }
    }
}
