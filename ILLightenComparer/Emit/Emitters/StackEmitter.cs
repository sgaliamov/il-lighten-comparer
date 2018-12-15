using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Emitters.Variables;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class StackEmitter
    {
        private readonly VariableLoader _loader;

        public StackEmitter(VariableLoader loader)
        {
            _loader = loader;
        }

        public ILEmitter Visit(IBasicVariable member, ILEmitter il, Label gotoNextMember)
        {
            var memberType = member.VariableType;
            if (memberType.IsNullable())
            {
                return LoadNullableMembers(il, true, false, member, gotoNextMember);
            }

            member.LoadAddress(_loader, il, Arg.X);

            return member.Load(_loader, il, Arg.Y);
        }

        public ILEmitter Visit(IArgumentsVariable variable, ILEmitter il, Label gotoNextMember)
        {
            var memberType = variable.VariableType;
            if (memberType.IsNullable())
            {
                return LoadNullableMembers(
                    il,
                    false,
                    variable.LoadContext,
                    variable,
                    gotoNextMember);
            }

            if (variable.LoadContext)
            {
                il.LoadArgument(Arg.Context);
            }

            variable.Load(_loader, il, Arg.X);
            variable.Load(_loader, il, Arg.Y);

            if (variable.LoadContext)
            {
                il.LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);
            }

            return il;
        }

        public ILEmitter Visit(IComparableVariable variable, ILEmitter il, Label gotoNextMember)
        {
            var memberType = variable.VariableType;
            var underlyingType = memberType.GetUnderlyingType();
            if (underlyingType.IsValueType)
            {
                return Visit((IBasicVariable)variable, il, gotoNextMember);
            }

            if (underlyingType.IsSealed)
            {
                variable.Load(_loader, il, Arg.X)
                        .Store(underlyingType, 0, out var x);

                return variable.Load(_loader, il, Arg.Y)
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
            variable.Load(_loader, il, Arg.X);

            return variable.Load(_loader, il, Arg.Y)
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

            member.Load(_loader, il, Arg.X).Store(memberType, 0, out var nullableX);
            member.Load(_loader, il, Arg.Y).Store(memberType, 1, out var nullableY);

            var getValueMethod = memberType.GetPropertyGetter(MethodName.Value);
            var underlyingType = memberType.GetUnderlyingType();

            il.CheckNullableValuesForNull(nullableX, nullableY, memberType, gotoNextMember);

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
    }
}
