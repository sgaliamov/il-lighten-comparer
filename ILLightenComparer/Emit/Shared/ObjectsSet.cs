using System.Collections.Concurrent;

namespace ILLightenComparer.Emit.Shared
{
    public sealed class ObjectsSet : ConcurrentDictionary<object, byte> { }
}
