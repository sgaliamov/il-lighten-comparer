using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Acceptors
{
    internal interface IArrayAcceptor : IAcceptor
    {
        Type ElementType { get; }
        MethodInfo GetItemMethod { get; }
        MethodInfo GetLengthMethod { get; }
    }
}
