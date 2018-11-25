﻿using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters;
using ILLightenComparer.Emit.Members;

namespace ILLightenComparer.Emit.Extensions
{
    internal static class ILEmitterExtensions
    {
        public static ILEmitter CallGetter(this ILEmitter il, PropertyMember member) =>
            il.Call(member, member.GetterMethod);

        public static ILEmitter Call(
            this ILEmitter il,
            Member member,
            MethodInfo methodInfo) =>
            il.Emit(
                member.OwnerType.IsValueType || member.OwnerType.IsSealed
                    ? OpCodes.Call
                    : OpCodes.Callvirt,
                methodInfo);

        public static ILEmitter LoadProperty(
            this ILEmitter il,
            PropertyMember member,
            ushort argumentIndex) =>
            il.LoadArgument(argumentIndex)
              .CallGetter(member);

        public static ILEmitter LoadField(
            this ILEmitter il,
            FieldMember member,
            ushort argumentIndex) =>
            il.LoadArgument(argumentIndex)
              .Emit(OpCodes.Ldfld, member.FieldInfo);
    }
}
