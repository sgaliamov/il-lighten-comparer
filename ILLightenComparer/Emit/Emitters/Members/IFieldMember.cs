using System;
using System.Reflection;

namespace ILLightenComparer.Emit.Emitters.Members
{
    internal interface IFieldMember : IVariable
    {
        Type DeclaringType { get; }
        FieldInfo FieldInfo { get; }
    }
}
