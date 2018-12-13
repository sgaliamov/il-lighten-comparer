using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Members;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        private readonly MemberLoader _loader;

        public StackEmitter(MemberLoader loader)
        {
            _loader = loader;
        }

        public ILEmitter Visit(IValueMember member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.VariableType;
            if (memberType.IsNullable())
            {
                return LoadNullableMembers(il, true, false, member, gotoNextMember);
            }

            member.LoadAddress(_loader, il, Arg.X);

            return member.Load(_loader, il, Arg.Y);
        }

        public ILEmitter Visit(IArgumentsMember member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.VariableType;
            if (memberType.IsNullable())
            {
                return LoadNullableMembers(
                    il,
                    false,
                    member.LoadContext,
                    member,
                    gotoNextMember);
            }


            if (member.LoadContext)
            {
                il.LoadArgument(Arg.Context);
            }

            member.Load(_loader, il, Arg.X);
            member.Load(_loader, il, Arg.Y);

            if (member.LoadContext)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }

            return il;
        }

        public ILEmitter Visit(IComparableMember member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.VariableType;
            var underlyingType = memberType.GetUnderlyingType();
            if (underlyingType.IsValueType)
            {
                return Visit((IValueMember)member, il, gotoNextMember);
            }

            if (underlyingType.IsSealed)
            {
                member.Load(_loader, il, Arg.X)
                      .Store(underlyingType, 0, out var x);

                return member.Load(_loader, il, Arg.Y)
                             .Store(underlyingType, 1, out var y)
                             .LoadLocal(x)
                             .Branch(OpCodes.Brtrue_S, out var call)
                             .LoadLocal(y)
                             .Emit(OpCodes.Brfalse_S, gotoNextMember)
                             .Return(-1)
                             .MarkLabel(call)
                             .LoadLocal(x)
                             .LoadLocal(y);
            }

            il.LoadArgument(Arg.Context);
            member.Load(_loader, il, Arg.X);

            return member.Load(_loader, il, Arg.Y)
                         .LoadArgument(Arg.SetX)
                         .LoadArgument(Arg.SetY);
        }

        private ILEmitter LoadNullableMembers(
            ILEmitter il,
            bool callable,
            bool loadContext,
            IAcceptor member,
            Label gotoNextMember)
        {
            var memberType = member.VariableType;

            member.Load(_loader, il, Arg.X)
                  .Store(memberType, 0, out var nullableX);

            member.Load(_loader, il, Arg.Y)
                  .Store(memberType, 1, out var nullableY);

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
                il.Store(underlyingType, out var x).LoadAddress(x);
            }

            il.LoadAddress(nullableY).Call(getValueMethod);

            if (loadContext)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }

            return il;
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
              .Branch(OpCodes.Brfalse_S, ifBothNull)
              .Return(-1)
              .MarkLabel(ifFirstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var getValues)
              .Return(1)
              .MarkLabel(getValues);
        }
    }
}
