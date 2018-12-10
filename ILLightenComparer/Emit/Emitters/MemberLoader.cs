using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class MemberLoader
    {
        public ILEmitter LoadMember(IPropertyMember member, ILEmitter il, ushort arg) =>
            il.LoadProperty(member, arg);

        public ILEmitter LoadMemberAddress(IPropertyMember member, ILEmitter il, ushort arg) =>
            il.LoadProperty(member, arg)
              .Store(member.MemberType.GetUnderlyingType(), out var local)
              .LoadAddress(local);

        public ILEmitter LoadMember(IFieldMember member, ILEmitter il, ushort arg) =>
            il.LoadField(member, arg);

        public ILEmitter LoadMemberAddress(IFieldMember member, ILEmitter il, ushort arg) =>
            il.LoadFieldAddress(member, arg);
    }
}
