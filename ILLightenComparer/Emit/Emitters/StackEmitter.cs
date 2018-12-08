using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        public ILEmitter Visit(ICallableField member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.MemberType;
            if (!memberType.IsNullable())
            {
                return il.LoadFieldAddress(member, Arg.X)
                         .LoadField(member, Arg.Y);
            }

            il.LoadField(member, Arg.X)
              .Store(memberType, 0, out var nullableX)
              .LoadField(member, Arg.Y)
              .Store(memberType, 1, out var nullableY);

            return LoadNullableMembers(il, true, false, memberType, nullableX, nullableY, gotoNextMember);
        }

        public ILEmitter Visit(ICallableProperty member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.MemberType;
            if (!memberType.IsNullable())
            {
                return il.LoadProperty(member, Arg.X)
                         .Store(memberType.GetUnderlyingType(), out var xAddress)
                         .LoadAddress(xAddress)
                         .LoadProperty(member, Arg.Y);
            }

            il.LoadProperty(member, Arg.X)
              .Store(memberType, 0, out var nullableX)
              .LoadProperty(member, Arg.Y)
              .Store(memberType, 1, out var nullableY);

            return LoadNullableMembers(il, true, false, memberType, nullableX, nullableY, gotoNextMember);
        }

        public ILEmitter Visit(IArgumentsField member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.MemberType;
            if (!memberType.IsNullable())
            {
                if (member.LoadContext)
                {
                    il.LoadArgument(Arg.Context);
                }

                return il.LoadField(member, Arg.X)
                         .LoadField(member, Arg.Y);
            }

            il.LoadField(member, Arg.X)
              .Store(memberType, 0, out var nullableX)
              .LoadField(member, Arg.Y)
              .Store(memberType, 1, out var nullableY);

            return LoadNullableMembers(
                il,
                false,
                member.LoadContext,
                memberType,
                nullableX,
                nullableY,
                gotoNextMember);
        }

        public ILEmitter Visit(IArgumentsProperty member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.MemberType;
            if (!memberType.IsNullable())
            {
                if (member.LoadContext)
                {
                    il.LoadArgument(Arg.Context);
                }

                return il.LoadProperty(member, Arg.X)
                         .LoadProperty(member, Arg.Y);
            }

            il.LoadProperty(member, Arg.X)
              .Store(memberType, 0, out var nullableX)
              .LoadProperty(member, Arg.Y)
              .Store(memberType, 1, out var nullableY);

            return LoadNullableMembers(
                il,
                false,
                member.LoadContext,
                memberType,
                nullableX,
                nullableY,
                gotoNextMember);
        }

        private static ILEmitter LoadNullableMembers(
            ILEmitter il,
            bool callable,
            bool loadContext,
            Type memberType,
            LocalBuilder nullableX,
            LocalBuilder nullableY,
            Label gotoNextMember)
        {
            var hasValueMethod = memberType.GetPropertyGetter(MethodName.HasValue);
            var getValueMethod = memberType.GetPropertyGetter(MethodName.Value);
            var underlyingType = memberType.GetUnderlyingType();

            CheckNullableValuesForNull(il, nullableX, nullableY, hasValueMethod, gotoNextMember);

            if (loadContext)
            {
                il.LoadArgument(Arg.Context);
            }

            il.LoadAddress(nullableX).Call(getValueMethod);

            if (callable)
            {
                il.Store(underlyingType, out var xAddress).LoadAddress(xAddress);
            }

            return il.LoadAddress(nullableY).Call(getValueMethod);
        }

        private static void CheckNullableValuesForNull(
            ILEmitter il,
            LocalBuilder nullableX,
            LocalBuilder nullableY,
            MethodInfo hasValueMethod,
            Label ifBothNull)
        {
            il.LoadAddress(nullableY)
              .Call(hasValueMethod)
              .Store(typeof(bool), out var secondHasValue)
              .LoadAddress(nullableX)
              .Call(hasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var ifFirstHasValue)
              .LoadLocal(secondHasValue)
              .Emit(OpCodes.Brfalse_S, ifBothNull)
              .Return(-1)
              .MarkLabel(ifFirstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var getValues)
              .Return(1)
              .MarkLabel(getValues);
        }
    }
}
