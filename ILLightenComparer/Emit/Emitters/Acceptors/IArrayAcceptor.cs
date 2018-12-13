using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IArrayAcceptor : IAcceptor
    {
        MethodInfo CountMethod { get; }
        MethodInfo GetItemMethod { get; }
    }
}
