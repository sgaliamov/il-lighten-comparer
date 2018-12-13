using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IArrayAcceptor : IAcceptor
    {
        MethodInfo GetLengthMethod { get; }
        MethodInfo GetItemMethod { get; }
    }
}
