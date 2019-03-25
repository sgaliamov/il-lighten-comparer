using System.Collections.Concurrent;

namespace ILLightenComparer.Shared
{
    internal sealed class ConcurrentSet<T> : ConcurrentDictionary<T, byte> { }
}
