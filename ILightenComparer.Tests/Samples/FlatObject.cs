using System;

namespace ILightenComparer.Tests.Samples
{
    internal class FlatObject : IFlatObject
    {
        public int Integer { get; set; }
        public string String { get; set; }
        public DateTime DateTime { get; set; }
        public double Double { get; set; }
        public bool Boolean { get; set; }
        public object Object { get; set; }
        public float Float { get; set; }
        public byte Byte { get; set; }
        public SampleEnum Enum { get; set; }
    }
}
