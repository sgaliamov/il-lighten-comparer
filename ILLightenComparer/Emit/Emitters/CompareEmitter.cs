using System;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Emit.Emitters.Acceptors;
using ILLightenComparer.Emit.Extensions;
using ILLightenComparer.Emit.Reflection;

namespace ILLightenComparer.Emit.Emitters
{
    internal sealed class CompareEmitter
    {
        private readonly BuilderContext _context;
        private readonly StackEmitter _stackEmitter = new StackEmitter();

        public CompareEmitter(BuilderContext context) => _context = context;

        public ILEmitter Visit(IDefaultAcceptor member, ILEmitter il)
        {
            // todo: maybe merge with IHierarchicalAcceptor
            var memberType = member.MemberType;
            var method = GetCompareToMethod(memberType);

            return member.Accept(_stackEmitter, il)
                         .Call(method)
                         .EmitReturnNotZero();
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il) =>
            member.Accept(_stackEmitter, il)
                  .Emit(OpCodes.Sub)
                  .EmitReturnNotZero();

        public ILEmitter Visit(INullableAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;

            member.Accept(_stackEmitter, il)
                  .DeclareLocal(memberType, out var n1, 0)
                  .DeclareLocal(memberType, out var n2, 1)
                  .DefineLabel(out var next);

            CheckValuesForNull(il, member, n1, n2, next);

            if (memberType.IsSmallIntegral())
            {
                il.LoadAddress(n1)
                  .Call(member.GetValueMethod)
                  .LoadAddress(n2)
                  .Call(member.GetValueMethod)
                  .Emit(OpCodes.Sub);
            }
            else
            {
                var method = GetCompareToMethod(memberType);

                il.LoadAddress(n1)
                  .Call(member.GetValueMethod)
                  .DeclareLocal(memberType.GetUnderlyingType(), out var local)
                  .Store(local)
                  .LoadAddress(local)
                  .LoadAddress(n2)
                  .Call(member.GetValueMethod)
                  .Call(method);
            }

            il.EmitReturnNotZero(next);

            return il;
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            var comparisonType = (int)_context.Configuration.StringComparisonType;

            return member.Accept(_stackEmitter, il)
                         .Emit(OpCodes.Ldc_I4_S, comparisonType) // todo: use short form for constants
                         .Call(Method.StringCompare)
                         .EmitReturnNotZero();
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            if (member.MemberType.IsValueType || member.MemberType.IsSealed)
            {
                throw new NotImplementedException();
            }

            return il;
        }

        private static MethodInfo GetCompareToMethod(Type memberType) =>
            memberType.GetCompareToMethod()
            ?? throw new ArgumentException(
                $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

        private static void CheckValuesForNull(
            ILEmitter il,
            INullableAcceptor member,
            LocalBuilder n1,
            LocalBuilder n2,
            Label next)
        {
            il.Store(n2)
              .LoadAddress(n2)
              // var secondHasValue = n2->HasValue
              .Call(member.HasValueMethod)
              .DeclareLocal(typeof(bool), out var secondHasValue)
              .Store(secondHasValue)
              // var n1 = &arg1
              .Store(n1)
              .LoadAddress(n1)
              // if n1->HasValue goto firstHasValue
              .Call(member.HasValueMethod)
              .Branch(OpCodes.Brtrue_S, out var firstHasValue)
              // if n2->HasValue goto returnZero
              .LoadLocal(secondHasValue)
              .Emit(OpCodes.Brfalse_S, next)
              // else return -1
              .Emit(OpCodes.Ldc_I4_M1)
              .Emit(OpCodes.Ret)
              // firstHasValue:
              .MarkLabel(firstHasValue)
              .LoadLocal(secondHasValue)
              .Branch(OpCodes.Brtrue_S, out var getValues)
              // return 1
              .Emit(OpCodes.Ldc_I4_1)
              .Emit(OpCodes.Ret)
              // getValues: load values
              .MarkLabel(getValues);
        }
    }
}
