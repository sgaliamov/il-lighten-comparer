using System;

namespace ILLightenComparer.Benchmarks.Models
{
    public sealed class FlatObject
    {
        public bool? BooleanField;
        public int IntegerField;
        public string StringField;

        public DateTime? DateTime { get; set; }
        public double Double { get; set; }
        public decimal Decimal { get; set; }
        public float? Float { get; set; }
        public byte Byte { get; set; }
        public string StringProperty { get; set; }
    }
}
