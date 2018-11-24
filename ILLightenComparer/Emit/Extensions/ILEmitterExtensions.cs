using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static ILEmitter CallGetter(this ILEmitter il, PropertyMember member) =>
            il.Call(member, member.GetterMethod);

        public static ILEmitter Call(this ILEmitter il, Member member, MethodInfo methodInfo) =>
            il.Emit(
                member.OwnerType.IsValueType || member.OwnerType.IsSealed
                    ? OpCodes.Call
                    : OpCodes.Callvirt,
                methodInfo);
    }
}
