using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static ILEmitter Call(this ILEmitter il, Member member, MethodInfo methodInfo) =>
            il.Emit(
                member.OwnerType.IsValueType || member.OwnerType.IsSealed
                    ? OpCodes.Call
                    : OpCodes.Callvirt,
                methodInfo);

        public static ILEmitter LoadProperty(this ILEmitter il, PropertyMember member, ushort argumentIndex)
        {
            if (member.OwnerType.IsValueType)
            {
                il.LoadArgumentAddress(argumentIndex);
            }
            else
            {
                il.LoadArgument(argumentIndex);
            }

            return il.Call(member, member.GetterMethod);
        }

        public static ILEmitter LoadPropertyAddress(this ILEmitter il, PropertyMember member, ushort argumentIndex) =>
            il.LoadProperty(member, argumentIndex)
              .DeclareLocal(member.GetterMethod.ReturnType.GetUnderlyingType(), out var local)
              .Store(local)
              .LoadAddress(local);

        public static ILEmitter LoadField(this ILEmitter il, FieldMember member, ushort argumentIndex) =>
            il.LoadArgument(argumentIndex)
              .Emit(OpCodes.Ldfld, member.FieldInfo);

        public static ILEmitter LoadFieldAddress(this ILEmitter il, FieldMember member, ushort argumentIndex)
        {
            if (member.OwnerType.IsValueType)
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
