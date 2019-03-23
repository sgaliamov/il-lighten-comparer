using System.Collections.Concurrent;

namespace ILLightenComparer.Shared
{
    public sealed class ConcurrentSet<T> : ConcurrentDictionary<T, byte> { }
}
