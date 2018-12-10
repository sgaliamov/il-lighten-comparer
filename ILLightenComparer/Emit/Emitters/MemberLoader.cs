using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class MemberLoader
    {
        public ILEmitter LoadMember(IPropertyMember member, ILEmitter il, ushort arg) => LoadProperty(il, member, arg);

        public ILEmitter LoadMemberAddress(IPropertyMember member, ILEmitter il, ushort arg) => LoadProperty(il, member, arg)
                                                                                                .Store(member.MemberType.GetUnderlyingType(), out var local)
                                                                                                .LoadAddress(local);

        public ILEmitter LoadMember(IFieldMember member, ILEmitter il, ushort arg) => LoadField(il, member, arg);

        public ILEmitter LoadMemberAddress(IFieldMember member, ILEmitter il, ushort arg) => LoadFieldAddress(il, member, arg);

        private static ILEmitter LoadProperty(ILEmitter il, IPropertyMember member, ushort argumentIndex)
        {
            if (member.DeclaringType.IsValueType)
            {
                il.LoadArgumentAddress(argumentIndex);
            }
            else
            {
                il.LoadArgument(argumentIndex);
            }

            return il.Call(member.GetterMethod);
        }

        private static ILEmitter LoadField(ILEmitter il, IFieldMember member, ushort argumentIndex) => il.LoadArgument(argumentIndex)
                                                                                                         .Emit(OpCodes.Ldfld, member.FieldInfo);

        private static ILEmitter LoadFieldAddress(ILEmitter il, IFieldMember member, ushort argumentIndex)
        {
            if (member.DeclaringType.IsValueType)
            {
                il.LoadArgumentAddress(argumentIndex);
            }
            else
            {
                il.LoadArgument(argumentIndex);
            }

            return il.Emit(OpCodes.Ldflda, member.FieldInfo);
        }
    }
}
