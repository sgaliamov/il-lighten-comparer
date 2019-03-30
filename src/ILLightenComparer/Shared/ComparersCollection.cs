using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ILLightenComparer.Shared
{
    internal sealed class ComparersCollection : ConcurrentDictionary<Type, object>
    {
        public ComparersCollection() { }
        public ComparersCollection(IEnumerable<KeyValuePair<Type, object>> collection) : base(collection) { }
    }
}
