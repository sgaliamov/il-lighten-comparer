using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyMember
    {
        Type OwnerType { get; }
        MethodInfo GetterMethod { get; }
    }
}
