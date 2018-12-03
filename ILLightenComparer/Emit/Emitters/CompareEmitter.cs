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
                  .DefineLabel(out var next)
                  .Store(memberType, 1, out var n2)
                  .Store(memberType, 0, out var n1);

            CheckNullableValuesForNull(il, member, n1, n2, next);

            if (memberType.GetUnderlyingType().IsSmallIntegral())
            {
                return il.LoadAddress(n1)
                         .Call(member.GetValueMethod)
                         .LoadAddress(n2)
                         .Call(member.GetValueMethod)
                         .Emit(OpCodes.Sub)
                         .EmitReturnNotZero(next);
            }

            var compareToMethod = memberType.GetCompareToMethod();
            if (compareToMethod != null)
            {
                return il.LoadAddress(n1)
                         .Call(member.GetValueMethod)
                         .Store(memberType.GetUnderlyingType(), out var local)
                         .LoadAddress(local)
                         .LoadAddress(n2)
                         .Call(member.GetValueMethod)
                         .Call(compareToMethod)
                         .EmitReturnNotZero(next);
            }

            // todo: test
            il.LoadArgument(0)
              .LoadAddress(n1)
              .LoadAddress(n2)
              .LoadArgument(3);

            return CompareComplex(il, memberType);
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

            member.LoadMembers(_stackEmitter, il.LoadArgument(0))
                  .LoadArgument(3);

            return CompareComplex(il, memberType);
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
                         .Call(compareToMethod) // todo: test for not sealed comparable member
                         .EmitReturnNotZero(next);
        }

        private ILEmitter CompareComplex(ILEmitter il, Type memberType)
        {
            if (memberType.IsValueType || memberType.IsSealed)
            {
                var compareMethod = _context.GetStaticCompareMethod(memberType);
                return il.Call(compareMethod)
                         .EmitReturnNotZero();
            }

            var contextCompare = Method.ContextCompare.MakeGenericMethod(memberType);

            return il.Emit(OpCodes.Call, contextCompare)
                     .EmitReturnNotZero();
        }

        private static void CheckNullableValuesForNull(
            ILEmitter il,
            INullableAcceptor member,
            LocalBuilder n1,
            LocalBuilder n2,
            Label next)
        {
            il.LoadAddress(n2)
              .Call(member.HasValueMethod)
              .Store(typeof(bool), out var secondHasValue)
              .LoadAddress(n1)
              .Call(member.HasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var firstHasValue)
              .LoadLocal(secondHasValue)
              .Emit(OpCodes.Brfalse_S, next)
              .Return(-1)
              .MarkLabel(firstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var getValues)
              .Return(1)
              .MarkLabel(getValues);
        }
    }
}
