using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Comparisons
{
    internal interface ICollectionComparison : IComparison
    {
        Type ElementType { get; }
        MethodInfo GetItemMethod { get; }
        MethodInfo GetLengthMethod { get; }
    }
}
