using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface INullableAcceptor : IAcceptor
    {
        Type MemberType { get; }
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
        MethodInfo CompareToMethod { get; }
    }
}
