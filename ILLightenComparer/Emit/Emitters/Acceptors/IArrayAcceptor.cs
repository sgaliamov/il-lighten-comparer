using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IArrayAcceptor : IAcceptor
    {
        MethodInfo GetItemMethod { get; }
        MethodInfo GetLengthMethod { get; }
    }
}
