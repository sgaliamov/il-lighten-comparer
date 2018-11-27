using System;
using System.Reflection;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;

namespace ILLightenComparer.Emit.Members
{
    internal abstract class FieldMember : IFieldMember
    {
        protected FieldMember(FieldInfo fieldInfo) =>
            FieldInfo = fieldInfo;

        public FieldInfo FieldInfo { get; }

        public Type MemberType => FieldInfo.FieldType;
        public Type OwnerType => FieldInfo.DeclaringType;

        public abstract ILEmitter Accept(StackEmitter stacker, ILEmitter il);
        public abstract ILEmitter Accept(CompareEmitter emitter, ILEmitter il);
    }
}
