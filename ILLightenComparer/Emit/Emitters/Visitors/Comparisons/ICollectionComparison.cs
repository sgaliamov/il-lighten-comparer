using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Visitors.Comparisons
{
    internal interface ICollectionComparison : IComparison
    {
        Type ElementType { get; }
        MethodInfo GetItemMethod { get; }
        MethodInfo GetLengthMethod { get; }
    }
}
