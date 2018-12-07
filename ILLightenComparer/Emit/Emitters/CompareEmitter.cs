using System;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly Context _context;
        private readonly StackEmitter _stackEmitter = new StackEmitter();

        public CompareEmitter(Context context) => _context = context;

        public ILEmitter Visit(IBasicAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;
            var method = memberType.GetCompareToMethod()
                         ?? throw new ArgumentException(
                             $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return member.LoadMembers(_stackEmitter, il)
                         .Call(method)
                         .EmitReturnNotZero();
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il) =>
            member.LoadMembers(_stackEmitter, il)
                  .Emit(OpCodes.Sub)
                  .EmitReturnNotZero();

        public ILEmitter Visit(INullableAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;

            member.LoadMembers(_stackEmitter, il)
                  .DefineLabel(out var gotoNextMember)
                  .Store(memberType, 1, out var nullableY)
                  .Store(memberType, 0, out var nullableX);

            CheckNullableValuesForNull(il, member, nullableX, nullableY, gotoNextMember);

            var underlyingType = memberType.GetUnderlyingType();
            if (underlyingType.IsSmallIntegral())
            {
                return il.LoadAddress(nullableX)
                         .Call(member.GetValueMethod)
                         .LoadAddress(nullableY)
                         .Call(member.GetValueMethod)
                         .Emit(OpCodes.Sub)
                         .EmitReturnNotZero(gotoNextMember);
            }

            // todo: test with comparable struct
            var compareToMethod = memberType.GetCompareToMethod();
            if (compareToMethod != null)
            {
                return il.LoadAddress(nullableX)
                         .Call(member.GetValueMethod)
                         .Store(underlyingType, out var local)
                         .LoadAddress(local)
                         .LoadAddress(nullableY)
                         .Call(member.GetValueMethod)
                         .Call(compareToMethod)
                         .EmitReturnNotZero(gotoNextMember);
            }

            il.LoadArgument(Arg.Context)
              .LoadAddress(nullableX)
              .Call(member.GetValueMethod)
              .LoadAddress(nullableY)
              .Call(member.GetValueMethod)
              .LoadArgument(Arg.SetX)
              .LoadArgument(Arg.SetY);

            return CompareComplex(il, underlyingType).EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;

            return member.LoadMembers(_stackEmitter, il)
                         .LoadConstant(comparisonType)
                         .Call(Method.StringCompare)
                         .EmitReturnNotZero();
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;

            il.LoadArgument(Arg.Context);
            member.LoadMembers(_stackEmitter, il)
                  .LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);

            return CompareComplex(il, memberType).EmitReturnNotZero();
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetCompareToMethod();

            return member.LoadMembers(_stackEmitter, il)
                         .Store(memberType, 1, out var l2)
                         .Store(memberType, 0, out var l1)
                         .LoadLocal(l1)
                         .Branch(OpCodes.Brtrue_S, out var call)
                         .LoadLocal(l2)
                         .Branch(OpCodes.Brfalse_S, out var next)
                         .Return(-1)
                         .MarkLabel(call)
                         .LoadLocal(l1)
                         .LoadLocal(l2)
                         .Call(compareToMethod) // todo: test for replacing not sealed comparable member
                         .EmitReturnNotZero(next);
        }

        public void EmitReferenceComparison(ILEmitter il)
        {
            il.LoadArgument(Arg.X) // x == y
              .LoadArgument(Arg.Y)
              .Branch(OpCodes.Bne_Un_S, out var checkY)
              .Return(0)
              .MarkLabel(checkY)
              // y != null
              .LoadArgument(Arg.Y)
              .Branch(OpCodes.Brtrue_S, out var checkX)
              .Return(1)
              .MarkLabel(checkX)
              // x != null
              .LoadArgument(Arg.X)
              .Branch(OpCodes.Brtrue_S, out var next)
              .Return(-1)
              .MarkLabel(next);
        }

        private ILEmitter CompareComplex(ILEmitter il, Type memberType)
        {
            if (memberType.IsValueType || memberType.IsSealed)
            {
                var compareMethod = _context.GetStaticCompareMethod(memberType);
                if (compareMethod != null)
                {
                    return il.Call(compareMethod);
                }
            }

            var contextCompare = Method.ContextCompare.MakeGenericMethod(memberType);

            return il.Emit(OpCodes.Call, contextCompare);
        }

        private static void CheckNullableValuesForNull(
            ILEmitter il,
            INullableAcceptor member,
            LocalBuilder n1,
            LocalBuilder n2,
            Label ifBothNull)
        {
            il.LoadAddress(n2)
              .Call(member.HasValueMethod)
              .Store(typeof(bool), out var secondHasValue)
              .LoadAddress(n1)
              .Call(member.HasValueMethod)
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
