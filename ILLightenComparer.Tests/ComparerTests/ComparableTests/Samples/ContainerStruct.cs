using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public struct ContainerStruct
    {
        public ComparableObject ComparableField;
        public ComparableStruct<EnumBig> ComparableStructField;
        public ComparableStruct<EnumSmall>? ComparableStructNullableField;

        public static IComparer<ContainerStruct> Comparer { get; } = new RelationalComparer();

        public ComparableChildObject ComparableProperty { get; set; }
        public ComparableStruct<decimal>? ComparableStructNullableProperty { get; set; }
        public ComparableStruct<string> ComparableStructProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ContainerStruct>
        {
            public int Compare(ContainerStruct x, ContainerStruct y)
            {
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

                compare = Comparer<ComparableChildObject>.Default.Compare(
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
