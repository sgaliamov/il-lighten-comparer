using System.Collections.Concurrent;

namespace ILLightenComparer.Emit.Shared
{
    public sealed class ConcurrentSet<T> : ConcurrentDictionary<T, byte> { }
}
