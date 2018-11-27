using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IFieldMember
    {
        Type OwnerType { get; }
        FieldInfo FieldInfo { get; }
    }
}