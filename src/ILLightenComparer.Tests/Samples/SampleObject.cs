using System.Diagnostics;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.Samples
{
    [DebuggerDisplay("{ToString()}")]
    public sealed class SampleObject<TMember>
    {
        public TMember Field;
        public TMember Property { get; set; }

        public override string ToString() => $"Object: {this.ToJson()}";
    }
}
