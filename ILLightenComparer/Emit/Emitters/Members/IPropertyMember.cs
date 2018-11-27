using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IPropertyMember
    {
        MethodInfo GetterMethod { get; }
        Type OwnerType { get; }
    }
}
