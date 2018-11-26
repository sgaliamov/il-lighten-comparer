using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static ILEmitter LoadProperty(this ILEmitter il, IPropertyMember member, ushort argumentIndex)
        {
            if (member.OwnerType.IsValueType)
            {
                il.LoadArgumentAddress(argumentIndex);
            }
            else
            {
                il.LoadArgument(argumentIndex);
            }

            return il.Call(member.OwnerType, member.GetterMethod);
        }

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
