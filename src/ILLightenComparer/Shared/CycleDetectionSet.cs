using System.Collections.Concurrent;

namespace ILLightenComparer.Shared
{
    internal sealed class CycleDetectionSet : ConcurrentDictionary<object, byte> { }
}
