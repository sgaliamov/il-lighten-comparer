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
            il.DefineLabel(out var gotoNextMember);
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetCompareToMethod()
                                  ?? throw new ArgumentException(
                                      $"{memberType.DisplayName()} does not have {MethodName.CompareTo} method.");

            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .Call(compareToMethod)
                         .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IIntegralAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember);

            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .Emit(OpCodes.Sub)
                         .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IStringAcceptor member, ILEmitter il)
        {
            var comparisonType = (int)_context.GetConfiguration(member.DeclaringType).StringComparisonType;
            il.DefineLabel(out var gotoNextMember);

            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .LoadConstant(comparisonType)
                         .Call(Method.StringCompare)
                         .EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IHierarchicalAcceptor member, ILEmitter il)
        {
            il.DefineLabel(out var gotoNextMember)
              .LoadArgument(Arg.Context);

            member.LoadMembers(_stackEmitter, gotoNextMember, il)
                  .LoadArgument(Arg.SetX)
                  .LoadArgument(Arg.SetY);

            return CompareComplex(il, member.MemberType.GetUnderlyingType()).EmitReturnNotZero(gotoNextMember);
        }

        public ILEmitter Visit(IComparableAcceptor member, ILEmitter il)
        {
            var memberType = member.MemberType;
            var compareToMethod = memberType.GetCompareToMethod();
            il.DefineLabel(out var gotoNextMember);

            // todo: test with comparable struct
            return member.LoadMembers(_stackEmitter, gotoNextMember, il)
                         .Store(memberType, 1, out var y)
                         .Store(memberType, 0, out var x)
                         .LoadLocal(x)
                         .Branch(OpCodes.Brtrue_S, out var call)
                         .LoadLocal(y)
                         .Emit(OpCodes.Brfalse_S, gotoNextMember)
                         .Return(-1)
                         .MarkLabel(call)
                         .LoadLocal(x)
                         .LoadLocal(y)
                         .Call(compareToMethod) // todo: test for replacing not sealed comparable member
                         .EmitReturnNotZero(gotoNextMember);
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
    }
}
