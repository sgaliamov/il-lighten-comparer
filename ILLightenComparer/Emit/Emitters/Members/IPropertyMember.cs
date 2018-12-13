using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyMember : IVariable
    {
        Type DeclaringType { get; }
        MethodInfo GetterMethod { get; }
    }
}
