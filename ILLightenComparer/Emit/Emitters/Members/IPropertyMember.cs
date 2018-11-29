using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyMember
    {
        Type DeclaringType { get; }
        MethodInfo GetterMethod { get; }
    }
}
