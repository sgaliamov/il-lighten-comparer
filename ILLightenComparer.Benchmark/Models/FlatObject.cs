using System;

namespace ILLightenComparer.Benchmark.Models
{
    public sealed class FlatObject
    {
        public DateTime? DateTime { get; set; }
        public double Double { get; set; }
        public bool Boolean { get; set; }
        public object Object { get; set; }
        public float? Float { get; set; }
        public byte Byte { get; set; }
        public int Integer { get; set; }
        public string String { get; set; }
    }
}
