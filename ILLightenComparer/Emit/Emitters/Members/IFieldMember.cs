using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IFieldMember
    {
        FieldInfo FieldInfo { get; }
        Type DeclaringType { get; }
    }
}
