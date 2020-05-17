using System.Diagnostics.CodeAnalysis;

namespace ILLightenComparer.Tests.Samples
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct SelfStruct<T>
    {
        public T Key;
        public T Value { get; set; }
        public SelfStruct<T> Self => this;
    }
}
