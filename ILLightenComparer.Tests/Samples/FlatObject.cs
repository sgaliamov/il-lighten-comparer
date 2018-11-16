using System;

namespace ILLightenComparer.Tests.Samples
{
    internal class FlatObject : IFlatObject
    {
        public bool BooleanField;
        public byte ByteField;
        public ushort ShortField;

        public DateTime DateTimeProperty { get; set; }
        public double DoubleProperty { get; set; }
        //public object ObjectProperty { get; set; }
        public float FloatProperty { get; set; }
        public SampleEnum EnumProperty { get; set; }
        public int IntegerProperty { get; set; }
        public string StringProperty { get; set; }
    }
}
