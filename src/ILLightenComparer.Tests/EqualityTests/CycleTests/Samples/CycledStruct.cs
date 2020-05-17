using System.Diagnostics.CodeAnalysis;

namespace ILLightenComparer.Tests.EqualityTests.CycleTests.Samples
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public struct CycledStruct
    {
        public sbyte? Property { get; set; }
        public CycledStructObject FirstObject { get; set; }
        public CycledStructObject SecondObject;
    }
}
