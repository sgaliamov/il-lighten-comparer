using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface ICollectionAcceptor : IAcceptor
    {
        MethodInfo CountMethod { get; }
        MethodInfo GetItemMethod { get; }
    }
}
