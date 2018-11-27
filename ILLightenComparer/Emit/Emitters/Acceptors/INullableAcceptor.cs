using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface INullableAcceptor : IAcceptor
    {
        MethodInfo GetValueMethod { get; }
        MethodInfo HasValueMethod { get; }
        MethodInfo CompareToMethod { get; }
    }
}
