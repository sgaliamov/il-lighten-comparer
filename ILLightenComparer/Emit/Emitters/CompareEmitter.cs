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

            //if (memberType.IsValueType || memberType.IsSealed)
            //{
            //    il.Emit(OpCodes.Ldarg_0); // todo: hash set will be hare
            //    member.Accept(_stackEmitter, il);

            //    var compareMethod = _context.GetStaticCompareMethod(memberType);

            //    il.Call(compareMethod).EmitReturnNotZero();
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}

            // todo: nullable can be also complex struct, not only primitive types, so it can be considered as hierarchical

            throw new NotSupportedException($"Unknown nullable case for {memberType.DisplayName()}.");
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;

            return member.LoadMembers(_stackEmitter, il)
                         .Emit(OpCodes.Ldc_I4_S, comparisonType) // todo: use short form for constants
                         .Call(Method.StringCompare)
                         .EmitReturnNotZero();
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;

            var compareToMethod = memberType.GetCompareToMethod(); // todo: member could implement not generic IComparable
            if (compareToMethod != null)
            {
                return member.LoadMembers(_stackEmitter, il)
                             .Store(memberType, 1, out var l2)
                             .Store(memberType, 0, out var l1)
                             .LoadLocal(l1)
                             .Branch(OpCodes.Brtrue_S, out var call)
                             .LoadLocal(l2)
                             .Branch(OpCodes.Brtrue_S, out var returnM1)
                             .Return(0)
                             .MarkLabel(returnM1)
                             .Return(-1)
                             .MarkLabel(call)
                             .LoadLocal(l1)
                             .LoadLocal(l2)
                             .Call(compareToMethod)
                             .EmitReturnNotZero();
            }

            if (memberType.IsValueType || memberType.IsSealed)
            {
                il.Emit(OpCodes.Ldarg_0); // todo: hash set will be hare
                member.LoadMembers(_stackEmitter, il);

                var compareMethod = _context.GetStaticCompareMethod(memberType);
                return il.Call(compareMethod)
                         .EmitReturnNotZero();
            }

            throw new NotSupportedException($"Unknown hierarchical case for {memberType.DisplayName()}.");
        }

        private static void CheckNullableValuesForNull(
            ILEmitter il,
            INullableAcceptor member,
            LocalBuilder n1,
            LocalBuilder n2,
            Label next)
        {
            il.LoadAddress(n2)
              // var secondHasValue = n2->HasValue
              .Call(member.HasValueMethod)
              .Store(typeof(bool), out var secondHasValue)
              .LoadAddress(n1)
              // if n1->HasValue goto firstHasValue
              .Call(member.HasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var firstHasValue)
              // if n2->HasValue goto returnZero
              .LoadLocal(secondHasValue)
              .Emit(OpCodes.Brfalse_S, next)
              .Return(-1)
              // firstHasValue:
              .MarkLabel(firstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var getValues)
              .Return(1)
              // getValues: load values
              .MarkLabel(getValues);
        }
    }
}
