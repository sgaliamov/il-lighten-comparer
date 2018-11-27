using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Emitters.Members;

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

            return il.Call(member.GetterMethod);
        }

        public static ILEmitter LoadField(this ILEmitter il, IFieldMember member, ushort argumentIndex) =>
            il.LoadArgument(argumentIndex)
              .Emit(OpCodes.Ldfld, member.FieldInfo);

        public static ILEmitter LoadFieldAddress(this ILEmitter il, IFieldMember member, ushort argumentIndex)
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

        public static ILEmitter EmitReturnNotZero(this ILEmitter il) =>
            il.DefineLabel(out var next)
              .EmitReturnNotZero(next);

        public static ILEmitter EmitReturnNotZero(this ILEmitter il, Label next) =>
            il.Emit(OpCodes.Stloc_0)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Brfalse_S, next)
              .Emit(OpCodes.Ldloc_0)
              .Emit(OpCodes.Ret)
              .MarkLabel(next);
    }
}
