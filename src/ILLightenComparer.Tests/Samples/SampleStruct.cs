using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Test class")]
    [DebuggerDisplay("{ToString()}")]
    public struct SampleStruct<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override string ToString() => $"Struct: {this.ToJson()}";
    }
}
