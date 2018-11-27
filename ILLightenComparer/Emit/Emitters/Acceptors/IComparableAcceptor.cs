using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IComparableAcceptor : IAcceptor
    {
        MethodInfo CompareToMethod { get; }
    }
}
